using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using software_engineering.Data;
using software_engineering.Filters;
using software_engineering.Models;

namespace software_engineering.Controllers
{
    [ClinicianOnly]  // Only Clinicians can access this controller
    public class ClinicianController : Controller
    {
        private readonly AppDBContext appDBcontext;

        public ClinicianController(AppDBContext context)
        {
            appDBcontext = context;
        }

        // GET: Clinician/Dashboard - Show all patients
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                // Get all non-admin users (patients)
                var patients = await appDBcontext.User
                    .Where(u => u.Role == Roles.user)
                    .ToListAsync();

                return View(patients);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading dashboard: {ex.Message}";
                return RedirectToAction("Login", "Users");
            }
        }

        // GET: Clinician/PatientDetails/5 - Show specific patient details and their pressure data
        public async Task<IActionResult> PatientDetails(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Patient not found.";
                return RedirectToAction(nameof(Dashboard));
            }

            try
            {
                // Get patient details
                var patient = await appDBcontext.User.FindAsync(id);
                if (patient == null || patient.Role != Roles.user)
                {
                    TempData["ErrorMessage"] = "Patient not found.";
                    return RedirectToAction(nameof(Dashboard));
                }

                // Get patient's pressure data
                var pressureData = await appDBcontext.PressureData
                    .Where(p => p.UserID == id)
                    .OrderByDescending(p => p.Timestamp)
                    .ToListAsync();

                // Create a view model to pass both patient and pressure data
                var viewModel = new
                {
                    Patient = patient,
                    PressureData = pressureData,
                    HighPressureCount = pressureData.Count(p => p.IsHighPressure),
                    FlaggedCount = pressureData.Count(p => p.FlaggedForReview)
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading patient details: {ex.Message}";
                return RedirectToAction(nameof(Dashboard));
            }
        }

        // GET: Clinician/DataDetail/5 - Show specific pressure data point
        public async Task<IActionResult> DataDetail(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Data not found.";
                return RedirectToAction(nameof(Dashboard));
            }

            try
            {
                var data = await appDBcontext.PressureData
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.DataID == id);

                if (data == null)
                {
                    TempData["ErrorMessage"] = "Data not found.";
                    return RedirectToAction(nameof(Dashboard));
                }

                return View(data);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading data: {ex.Message}";
                return RedirectToAction(nameof(Dashboard));
            }
        }

        // GET: Clinician/FlaggedData - Show all flagged data for review
        public async Task<IActionResult> FlaggedData()
        {
            try
            {
                var flaggedData = await appDBcontext.PressureData
                    .Include(p => p.User)
                    .Where(p => p.FlaggedForReview)
                    .OrderByDescending(p => p.Timestamp)
                    .ToListAsync();

                return View(flaggedData);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading flagged data: {ex.Message}";
                return RedirectToAction(nameof(Dashboard));
            }
        }

        // POST: Clinician/AddReviewNote - Add notes to flagged data
        [HttpPost]
        public async Task<IActionResult> AddReviewNote(int id, string reviewNotes)
        {
            try
            {
                var data = await appDBcontext.PressureData.FindAsync(id);
                if (data == null)
                {
                    return NotFound();
                }

                data.ReviewNotes = reviewNotes;
                data.FlaggedForReview = false; // Mark as reviewed
                await appDBcontext.SaveChangesAsync();

                TempData["SuccessMessage"] = "Review note added successfully.";
                return RedirectToAction(nameof(FlaggedData));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error adding review note: {ex.Message}";
                return RedirectToAction(nameof(FlaggedData));
            }
        }

        // POST: Clinician/AddComment - Add a new comment or reply to PressureData
        [HttpPost]
        public async Task<IActionResult> AddComment(int DataID, string Content, int? ParentCommentID)
        {
            if (string.IsNullOrWhiteSpace(Content))
            {
                TempData["ErrorMessage"] = "Comment cannot be empty.";
                return RedirectToAction("DataDetail", new { id = DataID });
            }

            // Get current user (clinician)
            var userEmail = User.Identity?.Name;
            var user = await appDBcontext.User.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("DataDetail", new { id = DataID });
            }

            var comment = new Comment
            {
                DataID = DataID,
                UserID = user.ID,
                Content = Content,
                ParentCommentID = ParentCommentID,
                CreatedAt = DateTime.Now
            };
            appDBcontext.Comments.Add(comment);
            await appDBcontext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Comment added.";
            return RedirectToAction("DataDetail", new { id = DataID });
        }

        // GET: Clinician/HighPressureAlerts - Show all high pressure alerts
        public async Task<IActionResult> HighPressureAlerts()
        {
            try
            {
                var alerts = await appDBcontext.PressureData
                    .Include(p => p.User)
                    .Where(p => p.IsHighPressure)
                    .OrderByDescending(p => p.Timestamp)
                    .ToListAsync();

                return View(alerts);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading alerts: {ex.Message}";
                return RedirectToAction(nameof(Dashboard));
            }
        }

        // GET: Clinician/ImportData - Show CSV import page
        public IActionResult ImportData()
        {
            return View();
        }

        // POST: Clinician/UploadCSV - Handle CSV file upload (sensor data format)
        [HttpPost]
        public async Task<IActionResult> UploadCSV(IFormFile file, int userId)
        {
            if (file == null || file.Length == 0)
            {
                TempData["ErrorMessage"] = "Please select a CSV file.";
                return RedirectToAction(nameof(ImportData));
            }

            if (!file.FileName.EndsWith(".csv"))
            {
                TempData["ErrorMessage"] = "Only CSV files are allowed.";
                return RedirectToAction(nameof(ImportData));
            }

            // Verify user exists and is a patient (role = user)
            var userRecord = await appDBcontext.User.FindAsync(userId);
            if (userRecord == null)
            {
                TempData["ErrorMessage"] = "Patient not found.";
                return RedirectToAction(nameof(ImportData));
            }
            if (userRecord.Role != Roles.user)
            {
                TempData["ErrorMessage"] = "Please select a valid patient (not admin or clinician).";
                return RedirectToAction(nameof(ImportData));
            }

            try
            {
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    var csvContent = await reader.ReadToEndAsync();
                    var lines = csvContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)
                        .Where(l => !string.IsNullOrWhiteSpace(l))
                        .ToArray();

                    if (lines.Length < 32)
                    {
                        TempData["ErrorMessage"] = "CSV file must have at least 32 rows (for one 32x32 matrix).";
                        return RedirectToAction(nameof(ImportData));
                    }

                    int importedCount = 0;
                    var timestamp = DateTime.Now;

                    // Process every 32 lines as one complete 32x32 sensor matrix reading
                    for (int matrixIndex = 0; matrixIndex + 31 < lines.Length; matrixIndex += 32)
                    {
                        try
                        {
                            float maxPressure = 0;
                            int cellsWithData = 0;
                            bool hasValidData = false;
                            var allValues = new List<float>();

                            // Read 32 rows (32 lines = 32x32 matrix)
                            for (int row = 0; row < 32; row++)
                            {
                                string line = lines[matrixIndex + row].Trim();
                                if (string.IsNullOrWhiteSpace(line))
                                    continue;

                                var columns = line.Split(',');
                                
                                for (int col = 0; col < columns.Length; col++)
                                {
                                    string val = columns[col].Trim();
                                    if (float.TryParse(val, out float value))
                                    {
                                        allValues.Add(value);
                                        if (value >= 10)
                                        {
                                            maxPressure = Math.Max(maxPressure, value);
                                            cellsWithData++;
                                            hasValidData = true;
                                        }
                                    }
                                }
                            }

                            // Create a record if we found some valid data
                            if (hasValidData)
                            {
                                // Calculate contact area percentage (based on cells with pressure >= 10)
                                float contactAreaPercentage = (cellsWithData / 1024f) * 100f; // 32*32 = 1024 cells

                                var pressureData = new PressureData
                                {
                                    UserID = userId,
                                    Timestamp = timestamp.AddMinutes(importedCount), // Offset each 32-line group by minutes
                                    PeakPressureIndex = maxPressure,
                                    ContactAreaPercentage = contactAreaPercentage,
                                    RawData = string.Join("\n", lines.Skip(matrixIndex).Take(32)), // Store all 32 lines as raw data
                                    IsHighPressure = maxPressure > 80,
                                    FlaggedForReview = maxPressure > 80,
                                    ReviewNotes = "",
                                    CreatedAt = DateTime.Now
                                };

                                appDBcontext.PressureData.Add(pressureData);
                                importedCount++;
                            }
                        }
                        catch (Exception ex)
                        {
                            // Log error and continue to next matrix
                            System.Diagnostics.Debug.WriteLine($"Error processing matrix at line {matrixIndex}: {ex.Message}");
                            continue;
                        }
                    }

                    if (importedCount > 0)
                    {
                        await appDBcontext.SaveChangesAsync();
                        TempData["SuccessMessage"] = $"Successfully imported {importedCount} pressure readings for {userRecord.Fullname}.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "No valid pressure data found in the CSV file.";
                    }
                    return RedirectToAction(nameof(Dashboard));
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error uploading file: {ex.Message}";
                return RedirectToAction(nameof(ImportData));
            }
        }

        // GET: Clear all flags (for testing)
        [HttpGet]
        [Route("Clinician/ClearAllFlags")]
        public async Task<IActionResult> ClearAllFlags()
        {
            try
            {
                var allFlagged = await appDBcontext.PressureData
                    .Where(p => p.FlaggedForReview == true)
                    .ToListAsync();

                foreach (var data in allFlagged)
                {
                    data.FlaggedForReview = false;
                    data.ReviewNotes = "";
                }

                appDBcontext.PressureData.UpdateRange(allFlagged);
                await appDBcontext.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Cleared {allFlagged.Count} flags from database.";
                return RedirectToAction(nameof(Dashboard));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error clearing flags: {ex.Message}";
                return RedirectToAction(nameof(Dashboard));
            }
        }

        // POST: Flag high pressure reading for review
        [HttpPost]
        [Route("Clinician/FlagHighPressure")]
        public async Task<IActionResult> FlagHighPressure([FromBody] dynamic request)
        {
            try
            {
                int dataId = request.dataId;
                string reviewNotes = request.reviewNotes;

                var pressureData = await appDBcontext.PressureData.FindAsync(dataId);
                if (pressureData == null)
                {
                    return BadRequest("Pressure data not found");
                }

                // If already flagged, replace with new review notes (don't accumulate)
                // This ensures we only see the current clinician's marked points
                pressureData.FlaggedForReview = true;
                pressureData.ReviewNotes = reviewNotes; // Overwrites previous notes
                pressureData.CreatedAt = DateTime.Now; // Update timestamp to show when it was last flagged

                appDBcontext.PressureData.Update(pressureData);
                await appDBcontext.SaveChangesAsync();

                return Ok(new { success = true, message = "Reading flagged for review" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }
}

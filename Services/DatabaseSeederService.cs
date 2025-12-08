using software_engineering.Data;
using software_engineering.Models;

namespace software_engineering.Services
{
    public class DatabaseSeederService
    {
        private readonly AppDBContext _context;

        public DatabaseSeederService(AppDBContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            try
            {
                // Check if data already exists to avoid duplicates
                if (_context.User.Any())
                {
                    System.Diagnostics.Debug.WriteLine("✅ Database already seeded, skipping...");
                    return;
                }

                // Add sample users
                var users = new List<Users>
                {
                    new Users
                    {
                        Fullname = "Admin User",
                        Email = "admin@test.com",
                        Password = "password123",
                        Role = Roles.Admin
                    },
                    new Users
                    {
                        Fullname = "Dr. Sarah Johnson",
                        Email = "clinician@test.com",
                        Password = "password123",
                        Role = Roles.clincian
                    },
                    new Users
                    {
                        Fullname = "John Doe",
                        Email = "patient1@test.com",
                        Password = "password123",
                        Role = Roles.user
                    },
                    new Users
                    {
                        Fullname = "Jane Smith",
                        Email = "patient2@test.com",
                        Password = "password123",
                        Role = Roles.user
                    }
                };

                _context.User.AddRange(users);
                await _context.SaveChangesAsync();

                // Get the patient users
                var patient1 = _context.User.FirstOrDefault(u => u.Email == "patient1@test.com");
                var patient2 = _context.User.FirstOrDefault(u => u.Email == "patient2@test.com");

                if (patient1 != null && patient2 != null)
                {
                    // Add sample pressure data
                    var pressureDataList = new List<PressureData>
                    {
                        // Patient 1 - Normal data
                        new PressureData
                        {
                            UserID = patient1.ID,
                            Timestamp = DateTime.Now.AddHours(-3),
                            RawData = "{}",
                            PeakPressureIndex = 165.2f,
                            ContactAreaPercentage = 38.1f,
                            IsHighPressure = false,
                            FlaggedForReview = false
                        },
                        // Patient 1 - High pressure alert
                        new PressureData
                        {
                            UserID = patient1.ID,
                            Timestamp = DateTime.Now.AddHours(-2),
                            RawData = "{}",
                            PeakPressureIndex = 280.5f,
                            ContactAreaPercentage = 52.3f,
                            IsHighPressure = true,
                            FlaggedForReview = false
                        },
                        // Patient 1 - Critical + flagged
                        new PressureData
                        {
                            UserID = patient1.ID,
                            Timestamp = DateTime.Now.AddHours(-1),
                            RawData = "{}",
                            PeakPressureIndex = 320.8f,
                            ContactAreaPercentage = 65.4f,
                            IsHighPressure = true,
                            FlaggedForReview = true
                        },
                        // Patient 1 - Recent normal
                        new PressureData
                        {
                            UserID = patient1.ID,
                            Timestamp = DateTime.Now,
                            RawData = "{}",
                            PeakPressureIndex = 155.3f,
                            ContactAreaPercentage = 35.5f,
                            IsHighPressure = false,
                            FlaggedForReview = false
                        },
                        // Patient 2 - Normal data
                        new PressureData
                        {
                            UserID = patient2.ID,
                            Timestamp = DateTime.Now.AddHours(-2),
                            RawData = "{}",
                            PeakPressureIndex = 142.7f,
                            ContactAreaPercentage = 32.1f,
                            IsHighPressure = false,
                            FlaggedForReview = false
                        },
                        // Patient 2 - High pressure
                        new PressureData
                        {
                            UserID = patient2.ID,
                            Timestamp = DateTime.Now.AddHours(-1),
                            RawData = "{}",
                            PeakPressureIndex = 295.4f,
                            ContactAreaPercentage = 58.2f,
                            IsHighPressure = true,
                            FlaggedForReview = false
                        },
                        // Patient 2 - Normal
                        new PressureData
                        {
                            UserID = patient2.ID,
                            Timestamp = DateTime.Now,
                            RawData = "{}",
                            PeakPressureIndex = 168.9f,
                            ContactAreaPercentage = 40.3f,
                            IsHighPressure = false,
                            FlaggedForReview = false
                        }
                    };

                    _context.PressureData.AddRange(pressureDataList);
                    await _context.SaveChangesAsync();

                    System.Diagnostics.Debug.WriteLine("✅ Database seeding completed successfully!");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error seeding database: {ex.Message}");
            }
        }
    }
}

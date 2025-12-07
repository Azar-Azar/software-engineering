using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace software_engineering.Models
{
    public class PressureData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DataID { get; set; }

        [Required]
        public int UserID { get; set; }

        [ForeignKey("UserID")]
        public Users User { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }

        [Required]
        public string RawData { get; set; } // JSON or CSV format of the 32x32 matrix

        [Required]
        public float PeakPressureIndex { get; set; }

        [Required]
        public float ContactAreaPercentage { get; set; }

        public bool IsHighPressure { get; set; } = false;

        public bool FlaggedForReview { get; set; } = false;

        public string ReviewNotes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}

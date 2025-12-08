using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace software_engineering.Models
{
    public class Comment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CommentID { get; set; }

        [Required]
        public int DataID { get; set; } // Foreign key to PressureData
        [ForeignKey("DataID")]
        public PressureData PressureData { get; set; }

        [Required]
        public int UserID { get; set; } // Foreign key to Users (author)
        [ForeignKey("UserID")]
        public Users User { get; set; }

        public int? ParentCommentID { get; set; } // For replies
        [ForeignKey("ParentCommentID")]
        public Comment ParentComment { get; set; }

        [Required]
        [StringLength(1000)]
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}

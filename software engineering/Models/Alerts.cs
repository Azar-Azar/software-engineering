using System.ComponentModel.DataAnnotations;

namespace software_engineering.Models
{
    public enum Type {Warning, Comments }
    public class Alerts
    {
        [Key]
        [Required]
        public int ID { get; set; }
        [Required]
        public int UserID { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }

        [Required]

        public Type type { get; set; }
        [Required]
        public bool Acknowledge { get; set; }

        public DateTime TimeStamp { get; set;}


    }
}

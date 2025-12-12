using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace software_engineering.Models
{
    public enum Roles { Admin, clincian, user }
    public class Users
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required(ErrorMessage = "Please select a full name")]
        [StringLength(50)]
        public string Fullname { get; set; }

        [Required(ErrorMessage = "Please enter a email")]
        [StringLength(150)]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter a Password")]
        [StringLength(50)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please select a role")]
        public Roles Role { get; set; }


    }
}

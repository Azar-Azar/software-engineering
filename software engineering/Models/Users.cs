namespace software_engineering.Models
{
    public class Users
    {
        public enum Roles { Admin,clincian,user }
        public int ID { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public Roles Role { get; set; }
        
    }
}

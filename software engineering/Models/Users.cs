﻿namespace software_engineering.Models
{
    public class Users
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public int Role { get; set; }
        public string Fullname { get; set; }
    }
}

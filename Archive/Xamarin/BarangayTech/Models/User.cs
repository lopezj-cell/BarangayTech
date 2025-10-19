using System;

namespace BarangayTech.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public UserRole Role { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastLoginDate { get; set; }
        
        // Resident-specific properties
        public string Address { get; set; }
        public string ResidentId { get; set; }
        
        // Admin-specific properties
        public string Department { get; set; }
        public string Position { get; set; }
    }

    public enum UserRole
    {
        Resident,
        Admin,
        SuperAdmin
    }
}
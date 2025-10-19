using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BarangayTech.Models;

namespace BarangayTech.Services.Auth
{
    public class AuthService
    {
        private static User _currentUser;
        private static List<User> _users;

        static AuthService()
        {
            InitializeSampleUsers();
        }

        public static User CurrentUser => _currentUser;
        public static bool IsLoggedIn => _currentUser != null;

        private static void InitializeSampleUsers()
        {
            _users = new List<User>
            {
                // Admin Users
                new User
                {
                    Id = 1,
                    Username = "admin",
                    Password = "admin123",
                    FullName = "Juan Dela Cruz",
                    Email = "admin@barangaytech.gov.ph",
                    ContactNumber = "(02) 123-4567",
                    Role = UserRole.Admin,
                    IsActive = true,
                    CreatedDate = DateTime.Now.AddMonths(-6),
                    Department = "Executive Office",
                    Position = "Barangay Captain"
                },
                new User
                {
                    Id = 2,
                    Username = "secretary",
                    Password = "secretary123",
                    FullName = "Carmen Lopez",
                    Email = "secretary@barangaytech.gov.ph",
                    ContactNumber = "(02) 123-4571",
                    Role = UserRole.Admin,
                    IsActive = true,
                    CreatedDate = DateTime.Now.AddMonths(-6),
                    Department = "Administrative Office",
                    Position = "Barangay Secretary"
                },
                new User
                {
                    Id = 3,
                    Username = "superadmin",
                    Password = "super123",
                    FullName = "System Administrator",
                    Email = "superadmin@barangaytech.gov.ph",
                    ContactNumber = "(02) 123-4500",
                    Role = UserRole.SuperAdmin,
                    IsActive = true,
                    CreatedDate = DateTime.Now.AddMonths(-12),
                    Department = "IT Department",
                    Position = "System Administrator"
                },
                
                // Resident Users
                new User
                {
                    Id = 4,
                    Username = "resident1",
                    Password = "resident123",
                    FullName = "Maria Santos",
                    Email = "maria.santos@email.com",
                    ContactNumber = "09123456789",
                    Role = UserRole.Resident,
                    IsActive = true,
                    CreatedDate = DateTime.Now.AddMonths(-3),
                    Address = "123 Main Street, Barangay Sample",
                    ResidentId = "RES-2024-001"
                },
                new User
                {
                    Id = 5,
                    Username = "resident2",
                    Password = "resident123",
                    FullName = "Roberto Garcia",
                    Email = "roberto.garcia@email.com",
                    ContactNumber = "09987654321",
                    Role = UserRole.Resident,
                    IsActive = true,
                    CreatedDate = DateTime.Now.AddMonths(-2),
                    Address = "456 Oak Avenue, Barangay Sample",
                    ResidentId = "RES-2024-002"
                },
                new User
                {
                    Id = 6,
                    Username = "resident3",
                    Password = "resident123",
                    FullName = "Ana Reyes",
                    Email = "ana.reyes@email.com",
                    ContactNumber = "09111222333",
                    Role = UserRole.Resident,
                    IsActive = true,
                    CreatedDate = DateTime.Now.AddMonths(-1),
                    Address = "789 Pine Street, Barangay Sample",
                    ResidentId = "RES-2024-003"
                }
            };
        }

        public static async Task<AuthResult> LoginAsync(string username, string password)
        {
            await Task.Delay(1000); // Simulate network delay

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    ErrorMessage = "Username and password are required."
                };
            }

            var user = _users.FirstOrDefault(u => 
                u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && 
                u.Password == password && 
                u.IsActive);

            if (user == null)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    ErrorMessage = "Invalid username or password."
                };
            }

            _currentUser = user;
            _currentUser.LastLoginDate = DateTime.Now;

            return new AuthResult
            {
                IsSuccess = true,
                User = user,
                Message = $"Welcome back, {user.FullName}!"
            };
        }

        public static void Logout()
        {
            _currentUser = null;
        }

        public static List<User> GetSampleCredentials()
        {
            return _users.Select(u => new User
            {
                Username = u.Username,
                Password = u.Password,
                FullName = u.FullName,
                Role = u.Role
            }).ToList();
        }

        public static bool HasPermission(string permission)
        {
            if (_currentUser == null) return false;

            switch (_currentUser.Role)
            {
                case UserRole.SuperAdmin:
                    return true; // Super admin has all permissions
                case UserRole.Admin:
                    return permission != "system_management"; // Admin has most permissions except system management
                case UserRole.Resident:
                    return permission == "view_events" || 
                           permission == "view_announcements" || 
                           permission == "view_services" ||
                           permission == "contact_officials"; // Residents have limited permissions
                default:
                    return false;
            }
        }
    }

    public class AuthResult
    {
        public bool IsSuccess { get; set; }
        public User User { get; set; }
        public string Message { get; set; }
        public string ErrorMessage { get; set; }
    }
}
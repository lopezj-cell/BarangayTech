namespace BarangayTech.Api.Models.DTOs
{
    // Login Request
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    // Registration Request
    public class RegisterRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Username { get; set; }
        public string? ContactNumber { get; set; }
        public string? Address { get; set; }
        public string Role { get; set; } = "Resident"; // Default to Resident
    }

    // Auth Response
    public class AuthResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? Error { get; set; }
        public UserData? User { get; set; }
        public string? IdToken { get; set; }
        public string? RefreshToken { get; set; }
    }

    // User Data (without sensitive info)
    public class UserData
    {
        public string? Id { get; set; }
        public string? Username { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? ContactNumber { get; set; }
        public string? Role { get; set; }
        public bool IsActive { get; set; }
        public bool EmailVerified { get; set; }
        public string? Address { get; set; }
        public string? ResidentId { get; set; }
        public string? Department { get; set; }
        public string? Position { get; set; }
        public string? PhotoUrl { get; set; }
    }

    // Change Password Request
    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    // Reset Password Request
    public class ResetPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
    }

    // Update Profile Request
    public class UpdateProfileRequest
    {
        public string? FullName { get; set; }
        public string? Username { get; set; }
        public string? ContactNumber { get; set; }
        public string? Address { get; set; }
        public string? PhotoUrl { get; set; }
    }
}

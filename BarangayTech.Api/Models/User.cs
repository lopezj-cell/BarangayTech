using Google.Cloud.Firestore;

namespace BarangayTech.Api.Models
{
    [FirestoreData]
    public class User
    {
        [FirestoreDocumentId]
        public string? Id { get; set; } // This will be the Firebase Auth UID

        [FirestoreProperty]
        public string Username { get; set; } = string.Empty;

        [FirestoreProperty]
        public string FullName { get; set; } = string.Empty;

        [FirestoreProperty]
        public string Email { get; set; } = string.Empty;

        [FirestoreProperty]
        public string? ContactNumber { get; set; }

        [FirestoreProperty]
        public string Role { get; set; } = "Resident"; // Resident, Admin, SuperAdmin

        [FirestoreProperty]
        public bool IsActive { get; set; } = true;

        [FirestoreProperty]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [FirestoreProperty]
        public DateTime? LastLoginDate { get; set; }

        // Resident-specific properties
        [FirestoreProperty]
        public string? Address { get; set; }

        [FirestoreProperty]
        public string? ResidentId { get; set; }

        // Admin-specific properties
        [FirestoreProperty]
        public string? Department { get; set; }

        [FirestoreProperty]
        public string? Position { get; set; }

        [FirestoreProperty]
        public bool EmailVerified { get; set; } = false;

        [FirestoreProperty]
        public string? PhotoUrl { get; set; }
    }

    public enum UserRole
    {
        Resident,
        Admin,
        SuperAdmin
    }
}

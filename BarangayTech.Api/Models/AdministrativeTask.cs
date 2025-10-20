using Google.Cloud.Firestore;

namespace BarangayTech.Api.Models
{
    [FirestoreData]
    public class AdministrativeTask
    {
        [FirestoreDocumentId]
        public string? Id { get; set; }

        [FirestoreProperty]
        public string Title { get; set; } = string.Empty;

        [FirestoreProperty]
        public string? Description { get; set; }

        [FirestoreProperty]
        public string? Category { get; set; } // e.g., Permit, Incident, Announcement

        [FirestoreProperty]
        public string Priority { get; set; } = "Medium"; // Low, Medium, High, Urgent

        [FirestoreProperty]
        public string Status { get; set; } = "Open"; // Open, InProgress, Completed, Cancelled

        [FirestoreProperty]
        public string? AssignedTo { get; set; } // staff username or id

        [FirestoreProperty]
        public DateTime? DueDate { get; set; }

        [FirestoreProperty]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [FirestoreProperty]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}

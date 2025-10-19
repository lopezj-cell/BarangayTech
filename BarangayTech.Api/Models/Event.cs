using Google.Cloud.Firestore;

namespace BarangayTech.Api.Models
{
    [FirestoreData]
    public class Event
    {
        [FirestoreDocumentId]
        public string? Id { get; set; }

        [FirestoreProperty]
        public string Title { get; set; } = string.Empty;

        [FirestoreProperty]
        public string Description { get; set; } = string.Empty;

        [FirestoreProperty]
        public DateTime Date { get; set; }

        [FirestoreProperty]
        public string? Location { get; set; }

        [FirestoreProperty]
        public string? ImageUrl { get; set; }

        [FirestoreProperty]
        public string? Organizer { get; set; }

        [FirestoreProperty]
        public bool IsPublic { get; set; } = true;

        [FirestoreProperty]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

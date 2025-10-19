using Google.Cloud.Firestore;

namespace BarangayTech.Api.Models
{
    [FirestoreData]
    public class Announcement
    {
        [FirestoreDocumentId]
        public string? Id { get; set; }

        [FirestoreProperty]
        public string Title { get; set; } = string.Empty;

        [FirestoreProperty]
        public string Content { get; set; } = string.Empty;

        [FirestoreProperty]
        public string? ImageUrl { get; set; }

        [FirestoreProperty]
        public DateTime DatePosted { get; set; } = DateTime.UtcNow;

        [FirestoreProperty]
        public string? Author { get; set; }

        [FirestoreProperty]
        public bool IsPinned { get; set; } = false;

        [FirestoreProperty]
        public string? Category { get; set; }
    }
}

using Google.Cloud.Firestore;

namespace BarangayTech.Api.Models
{
    [FirestoreData]
    public class Official
    {
        [FirestoreDocumentId]
        public string? Id { get; set; }

        [FirestoreProperty]
        public string Name { get; set; } = string.Empty;

        [FirestoreProperty]
        public string Position { get; set; } = string.Empty;

        [FirestoreProperty]
        public string? PhotoUrl { get; set; }

        [FirestoreProperty]
        public string? ContactNumber { get; set; }

        [FirestoreProperty]
        public string? Email { get; set; }

        [FirestoreProperty]
        public int DisplayOrder { get; set; } = 0;

        [FirestoreProperty]
        public bool IsActive { get; set; } = true;
    }
}

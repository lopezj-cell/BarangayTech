using Google.Cloud.Firestore;

namespace BarangayTech.Api.Models
{
    [FirestoreData]
    public class Service
    {
        [FirestoreDocumentId]
        public string? Id { get; set; }

        [FirestoreProperty]
        public string Name { get; set; } = string.Empty;

        [FirestoreProperty]
        public string Description { get; set; } = string.Empty;

        [FirestoreProperty]
        public string? IconUrl { get; set; }

        [FirestoreProperty]
        public List<string> Requirements { get; set; } = new();

        [FirestoreProperty]
        public string? ProcessingTime { get; set; }

        [FirestoreProperty]
        public decimal? Fee { get; set; }

        [FirestoreProperty]
        public bool IsActive { get; set; } = true;

        [FirestoreProperty]
        public string? Category { get; set; }
    }
}

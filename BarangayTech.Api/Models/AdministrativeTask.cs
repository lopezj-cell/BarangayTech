using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BarangayTech.Api.Models
{
    public class AdministrativeTask
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; } // e.g., Permit, Incident, Announcement
        public string Priority { get; set; } = "Medium"; // Low, Medium, High, Urgent
        public string Status { get; set; } = "Open"; // Open, InProgress, Completed, Cancelled
        public string? AssignedTo { get; set; } // staff username or id
        public DateTime? DueDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}

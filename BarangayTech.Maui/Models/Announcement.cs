using System;

namespace BarangayTech.Models
{
    public class Announcement
    {
        public int Id { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
        public DateTime DatePosted { get; set; }
    public string? Priority { get; set; } // High, Medium, Low
    public string? Category { get; set; }
    public string? PostedBy { get; set; }
        public bool IsUrgent { get; set; }
    public string? ImageUrl { get; set; }
    }
}
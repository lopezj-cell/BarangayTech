using System;

namespace BarangayTech.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
        public string ImageUrl { get; set; }
        public string Category { get; set; }
        public bool IsActive { get; set; }
        public string OrganizedBy { get; set; }
    }
}
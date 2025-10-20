using System;

namespace BarangayTech.Models
{
    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Requirements { get; set; }
        public string ProcessingTime { get; set; }
        public decimal Fee { get; set; }
        public string ContactPerson { get; set; }
        public string ContactNumber { get; set; }
        public string OfficeHours { get; set; }
        public string IconName { get; set; }
        public bool IsAvailable { get; set; }
    }
}
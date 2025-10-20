using System;
using System.Collections.Generic;
using BarangayTech.Models;

namespace BarangayTech.Services
{
    public class DataService
    {
        // Sample data methods (kept as fallback)
        public static List<Event> GetSampleEvents()
        {
            return new List<Event>
            {
                new Event
                {
                    Id = 1,
                    Title = "Community Clean-up Drive",
                    Description = "Join us for our monthly community clean-up drive. Bring your own cleaning materials and let's work together to keep our barangay clean and beautiful.",
                    Date = DateTime.Now.AddDays(2),
                    Location = "Barangay Plaza",
                    Category = "Community Service",
                    IsActive = true,
                    OrganizedBy = "Barangay Council"
                },
                new Event
                {
                    Id = 2,
                    Title = "Health and Wellness Seminar",
                    Description = "Learn about healthy living, nutrition, and disease prevention from our local health experts. Free health check-ups will also be available.",
                    Date = DateTime.Now.AddDays(7),
                    Location = "Barangay Hall",
                    Category = "Health",
                    IsActive = true,
                    OrganizedBy = "Health Center"
                },
                new Event
                {
                    Id = 3,
                    Title = "Christmas Festival",
                    Description = "Celebrate Christmas with the community! Enjoy food, games, performances, and gift-giving for children. Bring your family and friends!",
                    Date = DateTime.Now.AddDays(12),
                    Location = "Community Center",
                    Category = "Festival",
                    IsActive = true,
                    OrganizedBy = "Barangay Council"
                }
            };
        }

        public static List<Announcement> GetSampleAnnouncements()
        {
            return new List<Announcement>
            {
                new Announcement
                {
                    Id = 1,
                    Title = "URGENT: Water Service Interruption",
                    Content = "Water service will be temporarily interrupted tomorrow from 8:00 AM to 4:00 PM due to pipeline maintenance. Please store water in advance.",
                    DatePosted = DateTime.Now.AddMinutes(-30),
                    Priority = "Urgent",
                    Category = "Utilities",
                    PostedBy = "Barangay Captain",
                    IsUrgent = true
                },
                new Announcement
                {
                    Id = 2,
                    Title = "New Garbage Collection Schedule",
                    Content = "Starting next week, garbage collection will be on Tuesdays and Fridays. Please segregate your waste properly and place bins outside by 6:00 AM.",
                    DatePosted = DateTime.Now.AddHours(-2),
                    Priority = "High",
                    Category = "Sanitation",
                    PostedBy = "Sanitation Office",
                    IsUrgent = false
                },
                new Announcement
                {
                    Id = 3,
                    Title = "Free Medical Check-up Available",
                    Content = "The barangay health center is offering free medical check-ups for senior citizens and children. Available every Monday and Wednesday from 8:00 AM to 12:00 PM.",
                    DatePosted = DateTime.Now.AddDays(-1),
                    Priority = "Medium",
                    Category = "Health",
                    PostedBy = "Health Center",
                    IsUrgent = false
                }
            };
        }

        public static List<Service> GetSampleServices()
        {
            return new List<Service>
            {
                new Service
                {
                    Id = 1,
                    Name = "Birth Certificate",
                    Description = "Request for certified true copy of birth certificate",
                    Requirements = "Valid ID, Birth certificate copy",
                    ProcessingTime = "3-5 working days",
                    Fee = 50.00m,
                    ContactPerson = "Ms. Carmen Lopez",
                    ContactNumber = "(02) 123-4571",
                    OfficeHours = "8:00 AM - 5:00 PM",
                    IconName = "certificate",
                    IsAvailable = true
                },
                new Service
                {
                    Id = 2,
                    Name = "Business Permit",
                    Description = "Application for new business permit",
                    Requirements = "Business registration, Valid ID, Location clearance",
                    ProcessingTime = "7-10 working days",
                    Fee = 500.00m,
                    ContactPerson = "Mr. Pedro Gonzales",
                    ContactNumber = "(02) 123-4572",
                    OfficeHours = "8:00 AM - 5:00 PM",
                    IconName = "business",
                    IsAvailable = true
                },
                new Service
                {
                    Id = 3,
                    Name = "Health Services",
                    Description = "Medical check-ups and health consultations",
                    Requirements = "Valid ID, Health records (if any)",
                    ProcessingTime = "Same day",
                    Fee = 0.00m,
                    ContactPerson = "Dr. Maria Santos",
                    ContactNumber = "(02) 123-4568",
                    OfficeHours = "8:00 AM - 4:00 PM",
                    IconName = "health",
                    IsAvailable = true
                }
            };
        }

        public static List<Official> GetSampleOfficials()
        {
            return new List<Official>
            {
                new Official
                {
                    Id = 1,
                    Name = "Hon. Juan Dela Cruz",
                    Position = "Barangay Captain",
                    ContactNumber = "(02) 123-4567",
                    Email = "captain@barangaytech.gov.ph",
                    Department = "Executive Office",
                    OfficeHours = "8:00 AM - 5:00 PM"
                },
                new Official
                {
                    Id = 2,
                    Name = "Hon. Maria Santos",
                    Position = "Kagawad - Health & Sanitation",
                    ContactNumber = "(02) 123-4568",
                    Email = "health@barangaytech.gov.ph",
                    Department = "Health Department",
                    OfficeHours = "8:00 AM - 5:00 PM"
                },
                new Official
                {
                    Id = 3,
                    Name = "Hon. Roberto Garcia",
                    Position = "Kagawad - Public Safety",
                    ContactNumber = "(02) 123-4569",
                    Email = "safety@barangaytech.gov.ph",
                    Department = "Public Safety",
                    OfficeHours = "8:00 AM - 5:00 PM"
                },
                new Official
                {
                    Id = 4,
                    Name = "Ms. Carmen Lopez",
                    Position = "Barangay Secretary",
                    ContactNumber = "(02) 123-4571",
                    Email = "secretary@barangaytech.gov.ph",
                    Department = "Administrative Office",
                    OfficeHours = "8:00 AM - 5:00 PM"
                }
            };
        }
    }
}
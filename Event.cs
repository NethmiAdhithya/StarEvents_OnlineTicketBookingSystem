using System.ComponentModel.DataAnnotations;
using System;

namespace StarEvents.Models
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }

        [Required]
        public string Title { get; set; }
        public string Description { get; set; }

        // ... (other properties)

        // --- NEW PROPERTY TO STORE IMAGE PATH ---
        public string? ImagePath { get; set; }
        // ----------------------------------------

        [Required]
        public DateTime EventDate { get; set; }
        public TimeSpan EventTime { get; set; }

        [Required]
        public decimal TicketPrice { get; set; }
        public int TotalTickets { get; set; }
        public int AvailableTickets { get; set; }

        // Foreign Keys
        public string OrganizerId { get; set; }
        public int VenueId { get; set; }
        public int CategoryId { get; set; }

        // Navigation properties
        public virtual User Organizer { get; set; }
        public virtual Venue Venue { get; set; }
        public virtual EventCategory Category { get; set; }
        public object? UserId { get; internal set; }
        public string Status { get; internal set; }
    }
}
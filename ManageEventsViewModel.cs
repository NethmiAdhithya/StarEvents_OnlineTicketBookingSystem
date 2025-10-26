using StarEvents.Models;
using System.ComponentModel.DataAnnotations;

namespace StarEvents.ViewModels
{
    public class EventManagementItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string OrganizerName { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan Time { get; set; }

        public string Location { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal TicketPrice { get; set; }

        // Status can be "Pending", "Approved", "Rejected", "Draft"
        public string Status { get; set; }

        // --- NEW PROPERTY FOR IMAGE DISPLAY ---
        public string? ImagePath { get; set; }
        // --------------------------------------

        // --- FIX: ADD MISSING PROPERTIES HERE ---
        public int TicketsSold { get; set; }
        public int TotalTickets { get; set; }
        // ------------------------------------------
    }

    public class ManageEventsViewModel
    {
        public List<EventManagementItem> Events { get; set; } = new List<EventManagementItem>();
        public string SearchTerm { get; set; }
        public string FilterStatus { get; set; } // e.g., "Pending", "All"
    }
}
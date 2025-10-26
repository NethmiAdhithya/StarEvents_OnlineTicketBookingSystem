using System.ComponentModel.DataAnnotations;
using StarEvents.Models; // Assuming your Event model is here

namespace StarEvents.ViewModels
{
    public class BookingViewModel
    {
        // Properties to display event details on the booking page
        public int EventId { get; set; }
        public string Title { get; set; }
        public string VenueName { get; set; }
        public DateTime EventDate { get; set; }
        public TimeSpan EventTime { get; set; }
        public decimal TicketPrice { get; set; }
        public int AvailableTickets { get; set; }

        // Property for user input (the core of the booking form)
        [Required(ErrorMessage = "Please enter the number of tickets.")]
        [Range(1, int.MaxValue, ErrorMessage = "You must book at least 1 ticket.")]
        public int Quantity { get; set; }
        public string AttendeeEmail { get; internal set; }
        public string AttendeeName { get; internal set; }
    }
}
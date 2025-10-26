using StarEvents.Models;
using System.ComponentModel.DataAnnotations;
using System; // Make sure this is included for DateTime/TimeSpan

namespace StarEvents.ViewModels
{
    public class BookTicketViewModel
    {
        // --- Data to Display (from Event Model) ---
        public int EventId { get; set; }
        public string Title { get; set; }
        public decimal TicketPrice { get; set; }
        public int AvailableTickets { get; set; }
        public string ImagePath { get; set; }
        public string VenueName { get; set; }
        public DateTime EventDate { get; set; }
        public TimeSpan EventTime { get; set; }

        // --- Data to Capture (Step 1: Quantity) ---
        [Required(ErrorMessage = "Please select a ticket quantity.")]
        [Range(1, int.MaxValue, ErrorMessage = "You must book at least 1 ticket.")]
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        // Calculated on server/client
        public decimal TotalAmount { get; set; }

        // --- Data to Capture (Step 2: Attendee Details) ---
        [Required(ErrorMessage = "Please enter the full name for the ticket.")]
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string AttendeeName { get; set; }

        [Required(ErrorMessage = "Please enter an email address.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(100)]
        [Display(Name = "Email Address")]
        public string AttendeeEmail { get; set; }

        // --- Data to Capture (Step 3: Payment Method) ---
        [Required(ErrorMessage = "Please select a payment method.")]
        public string PaymentMethod { get; set; }
    }
}
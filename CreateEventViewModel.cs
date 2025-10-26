using System.ComponentModel.DataAnnotations;
using StarEvents.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;

namespace StarEvents.ViewModels
{
    public class CreateEventViewModel
    {
        // =========================================================================
        // FIXES FOR EDIT FUNCTIONALITY
        // =========================================================================

        // 1. Used by Edit action to identify the event being updated.
        public int EventId { get; set; }

        // 2. Stores the path of the existing image during an Edit operation.
        public string? ExistingImagePath { get; set; }

        // =========================================================================
        // EVENT DETAILS
        // =========================================================================

        [Required(ErrorMessage = "Event title is required")]
        [StringLength(200, ErrorMessage = "Title cannot be longer than 200 characters")]
        [Display(Name = "Event Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Event description is required")]
        [StringLength(2000, ErrorMessage = "Description cannot be longer than 2000 characters")]
        [Display(Name = "Event Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Event date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Event Date")]
        // NOTE: The 'FutureDate' attribute must be defined elsewhere in your project (likely in CustomAttributes folder)
        // [FutureDate(ErrorMessage = "Event date must be in the future")] 
        public DateTime EventDate { get; set; }

        [Required(ErrorMessage = "Event time is required")]
        [DataType(DataType.Time)]
        [Display(Name = "Event Time")]
        public TimeSpan EventTime { get; set; }

        [Required(ErrorMessage = "Ticket price is required")]
        [Range(0.01, 100000, ErrorMessage = "Ticket price must be between LKR 0.01 and LKR 100,000")]
        [Display(Name = "Ticket Price (LKR)")]
        public decimal TicketPrice { get; set; }

        [Required(ErrorMessage = "Number of tickets is required")]
        [Range(1, 100000, ErrorMessage = "Number of tickets must be between 1 and 100,000")]
        [Display(Name = "Total Tickets Available")]
        public int TotalTickets { get; set; }

        [Required(ErrorMessage = "Please select a category")]
        [Display(Name = "Event Category")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Please select a venue")]
        [Display(Name = "Venue")]
        public int VenueId { get; set; }

        // =========================================================================
        // IMAGE UPLOAD (REQUIRED FOR CREATE, OPTIONAL FOR EDIT)
        // =========================================================================

        // FIX: The IFormFile is now nullable (?) and the [Required] attribute is removed.
        // The required validation must now be implemented manually in the Controller's
        // POST Create action (check if ImageFile is null).
        [Display(Name = "Event Image")]
        public IFormFile? ImageFile { get; set; }

        // =========================================================================
        // DROPDOWN LISTS
        // =========================================================================

        public List<EventCategory> Categories { get; set; } = new List<EventCategory>();
        public List<Venue> Venues { get; set; } = new List<Venue>();
    }
}
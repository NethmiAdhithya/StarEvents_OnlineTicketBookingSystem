using StarEvents.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http; // IMPORTANT: Required for IFormFile

namespace StarEvents.ViewModels
{
    public class EditEventViewModel
    {
        // Key field
        public int EventId { get; set; }

        // Event Fields
        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EventDate { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan EventTime { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        [DataType(DataType.Currency)]
        public decimal TicketPrice { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Total tickets must be at least 1.")]
        public int TotalTickets { get; set; }

        // Foreign Keys (Required for form submission)
        [Required(ErrorMessage = "Please select a venue.")]
        [Display(Name = "Venue")]
        public int VenueId { get; set; }

        [Required(ErrorMessage = "Please select a category.")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        // ---------------------------------------------
        // --- ADDED PROPERTIES FOR IMAGE HANDLING ---
        // ---------------------------------------------

        // Stores the path of the existing image for display and rollback on validation error
        public string? ExistingImagePath { get; set; }

        // Used to bind the new file uploaded via the form
        [Display(Name = "Upload New Event Image")]
        public IFormFile? NewImageFile { get; set; }

        // ---------------------------------------------

        // Dropdown data for the view
        public List<EventCategory>? Categories { get; set; }
        public List<Venue>? Venues { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
using StarEvents.Models;

namespace StarEvents.ViewModels
{
    public class EventCreateViewModel
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [FutureDate(ErrorMessage = "Event date must be in the future.")]
        public DateTime EventDate { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan EventTime { get; set; }

        [Required]
        [Range(0.01, 100000)]
        public decimal TicketPrice { get; set; }

        [Required]
        [Range(1, 100000)]
        public int TotalTickets { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Required]
        [Display(Name = "Venue")]
        public int VenueId { get; set; }

        public List<EventCategory> Categories { get; set; } = new List<EventCategory>();
        public List<Venue> Venues { get; set; } = new List<Venue>();
    }

    // Custom validation attribute for future dates
    public class FutureDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is DateTime date)
            {
                return date > DateTime.Today;
            }
            return false;
        }
    }
}
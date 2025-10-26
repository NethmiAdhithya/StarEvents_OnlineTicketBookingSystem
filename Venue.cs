using System.ComponentModel.DataAnnotations;

namespace StarEvents.Models
{
    public class Venue
    {
        [Key]
        public int VenueId { get; set; }

        [Required]
        [StringLength(100)]
        public string VenueName { get; set; }

        [Required]
        [StringLength(255)]
        public string Address { get; set; }

        [Required]
        [StringLength(50)]
        public string City { get; set; }

        [Required]
        public int Capacity { get; set; }

        [Phone]
        public string ContactPhone { get; set; }

        [EmailAddress]
        public string ContactEmail { get; set; }

        [StringLength(500)]
        public string Facilities { get; set; }

        public bool IsActive { get; set; } = true;

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<Event> Events { get; set; }
        public virtual User CreatedByUser { get; set; }
        public object? UserId { get; internal set; }

        // Remove this duplicate property
        // public string UserId { get; set; }
        // public virtual User User { get; set; }
    }
}
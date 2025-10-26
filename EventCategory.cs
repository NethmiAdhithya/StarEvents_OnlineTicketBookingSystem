using System.ComponentModel.DataAnnotations;

namespace StarEvents.Models
{
    public class EventCategory
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(50)]
        public string CategoryName { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<Event> Events { get; set; }
    }
}
using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace StarEvents.Models
{
    public class User : IdentityUser
    {
        public string? City { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [StringLength(20)]
        public string Role { get; set; } = "Customer";

        public DateTime DateJoined { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        public DateTime? LastLogin { get; set; } // Nullable DateTime

        // Navigation properties
        // Assuming you have a 'Booking' model
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

        // For organizers (assuming the 'Event' model links back to the user)
        public virtual ICollection<Event> OrganizedEvents { get; set; } = new List<Event>();

        // For admin/user created venues (assuming the 'Venue' model links back to the user)
        public virtual ICollection<Venue> CreatedVenues { get; set; } = new List<Venue>();

        // Computed property for full name
        public string FullName => $"{FirstName} {LastName}";
    }
}
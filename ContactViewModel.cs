using System.ComponentModel.DataAnnotations;

namespace StarEvents.ViewModels
{
    public class ContactViewModel
    {
        public ContactInfo ContactInfo { get; set; }
        public ContactForm ContactForm { get; set; }
    }

    public class ContactInfo
    {
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string BusinessHours { get; set; }
    }

    public class ContactForm
    {
        [Required(ErrorMessage = "Please enter your name")]
        [StringLength(50, ErrorMessage = "Name cannot be longer than 50 characters")]
        [Display(Name = "Full Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter your email address")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Please enter a valid phone number")]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Please select a subject")]
        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Please enter your message")]
        [StringLength(1000, ErrorMessage = "Message cannot be longer than 1000 characters")]
        [Display(Name = "Message")]
        public string Message { get; set; }

        // For spam protection
        public string Honeypot { get; set; }
    }
}
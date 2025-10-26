// Inside StarEvents.ViewModels/UserDetailsViewModel.cs

using System.ComponentModel.DataAnnotations;

namespace StarEvents.ViewModels
{
    public class UserDetailsViewModel
    {
        public string Id { get; set; }

        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        // This is crucial for showing the current role
        public string Role { get; set; }
    }
}
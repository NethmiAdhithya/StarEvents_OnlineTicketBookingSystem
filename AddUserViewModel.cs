// StarEvents.ViewModels/AddUserViewModel.cs (Modified)

using System.ComponentModel.DataAnnotations;

namespace StarEvents.ViewModels
{
    public class AddUserViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")] // <--- NEW PROPERTY
        public string LastName { get; set; } // <--- NEW PROPERTY

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        // ... (rest of the properties remain unchanged) ...
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "User Role")]
        public string SelectedRole { get; set; }
    }
}
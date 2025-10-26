// Inside StarEvents.ViewModels/EditUserViewModel.cs

using System.ComponentModel.DataAnnotations;

namespace StarEvents.ViewModels
{
    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        // Fields for Role Management
        [Required(ErrorMessage = "Please select a role.")]
        [Display(Name = "Role")]
        public string SelectedRole { get; set; }

        // Used to track the original role to avoid unnecessary database updates
        public string CurrentRole { get; set; }
    }
}
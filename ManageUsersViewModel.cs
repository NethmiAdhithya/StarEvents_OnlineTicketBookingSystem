// C:\Adhithya\TOP-UP\Assignments\11.09_AD_CourseWork-02\StarEvents\ViewModels\ManageUsersViewModel.cs

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StarEvents.ViewModels
{
    // UserListItemViewModel (Definition 1)
    public class UserListItemViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Username")]
        public string UserName { get; set; }

        public string Email { get; set; }
        public string Role { get; set; }
    }

    // ManageUsersViewModel (Definition 2)
    public class ManageUsersViewModel
    {
        public List<UserListItemViewModel> Users { get; set; } = new List<UserListItemViewModel>();

        // Use 'public set' for model binding compatibility
        public string SearchTerm { get; set; }
    }
}
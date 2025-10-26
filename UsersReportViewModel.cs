// StarEvents/ViewModels/UsersReportViewModel.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace StarEvents.ViewModels
{
    public class UsersReportViewModel
    {
        [Display(Name = "Name")]
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

        [Display(Name = "Registration Date")]
        [DataType(DataType.Date)]
        public DateTime RegistrationDate { get; set; }
    }
}
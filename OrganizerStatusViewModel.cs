// StarEvents.ViewModels/OrganizerStatusViewModel.cs

namespace StarEvents.ViewModels
{
    public class OrganizerStatusViewModel
    {
        public string OrganizerName { get; set; }
        public string Email { get; set; }
        public string VerificationStatus { get; set; } // e.g., "Pending", "Approved", "Rejected"
        public DateTime? SubmissionDate { get; set; }
        public string AdminNotes { get; set; } // Notes if rejected or approved
        public string StatusColorClass => VerificationStatus switch
        {
            "Approved" => "text-success",
            "Pending" => "text-warning",
            "Rejected" => "text-danger",
            _ => "text-secondary",
        };
    }
}
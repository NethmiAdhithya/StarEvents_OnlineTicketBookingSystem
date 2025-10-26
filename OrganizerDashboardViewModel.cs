// StarEvents.ViewModels/OrganizerDashboardViewModel.cs

namespace StarEvents.ViewModels
{
    public class OrganizerDashboardViewModel
    {
        public int TotalEvents { get; set; }
        public int ApprovedEvents { get; set; }
        public int PendingEvents { get; set; }
        public int TotalTicketsSold { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AttendanceRate { get; set; } // For overall organizer performance
        public List<EventManagementItem> RecentEvents { get; set; } = new List<EventManagementItem>();
        public int TicketsSold { get; internal set; }
        public int UpcomingEvents { get; internal set; }
    }

    // Reuse MyEventItem or EventManagementItem for simplicity
    
}
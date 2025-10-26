// StarEvents.ViewModels/SalesReportsViewModel.cs

namespace StarEvents.ViewModels
{
    public class SalesReportsViewModel
    {
        public List<ReportEventItem> Events { get; set; } = new List<ReportEventItem>();
        public DateTime StartDate { get; set; } = DateTime.Today.AddMonths(-1);
        public DateTime EndDate { get; set; } = DateTime.Today;
        public int SelectedEventId { get; set; }

        // Report Data
        public decimal TotalRevenue { get; set; }
        public int TotalTicketsSold { get; set; }
        public Dictionary<DateTime, int> SalesTrend { get; set; } = new Dictionary<DateTime, int>();

        // Simplified Attendee Demographics
        public Dictionary<string, int> AttendeeCityBreakdown { get; set; } = new Dictionary<string, int>();
    }

    public class ReportEventItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }
}
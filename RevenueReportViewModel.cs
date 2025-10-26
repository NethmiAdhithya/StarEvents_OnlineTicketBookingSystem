namespace StarEvents.ViewModels
{
    // NOTE: The definition for 'ReportEventItem' has been REMOVED from this file
    // to resolve the CS0101/CS0229 errors. It must be defined in one other file
    // within this project (e.g., SalesReportsViewModel.cs or a separate common file)
    // for the rest of this code to compile.

    /// <summary>
    /// ViewModel for the Organizer Revenue Report view. Contains filters, summary data,
    /// financial breakdowns, and attendance metrics.
    /// </summary>
    public class RevenueReportViewModel
    {
        // --- Filters ---
        public List<ReportEventItem> Events { get; set; } = new List<ReportEventItem>();
        public DateTime StartDate { get; set; } = DateTime.Today.AddMonths(-1);
        public DateTime EndDate { get; set; } = DateTime.Today;
        public int SelectedEventId { get; set; }

        // --- Summary Data (High-level totals) ---
        public decimal TotalRevenue { get; set; }
        public int TotalTicketsSold { get; set; }
        public int TotalTicketsAvailable { get; set; } // Total capacity of reported events
        public decimal OverallAttendanceRate { get; set; } // (TotalTicketsSold / TotalTicketsAvailable) * 100

        // --- Detailed Financial Breakdown ---
        // Dictionary for Revenue by Event/Title (Event Title, Total Revenue)
        public Dictionary<string, decimal> RevenueByEvent { get; set; } = new Dictionary<string, decimal>();

        // Dictionary for Revenue by Payment Method (e.g., "Card", "Cash", Total Revenue)
        public Dictionary<string, decimal> RevenueByPaymentMethod { get; set; } = new Dictionary<string, decimal>();

        // --- Detailed Attendance Metrics ---
        // List to show per-event attendance metrics
        public List<AttendanceSummaryItem> AttendanceSummary { get; set; } = new List<AttendanceSummaryItem>();
    }

    /// <summary>
    /// Represents the detailed attendance metrics for a single event.
    /// </summary>
    public class AttendanceSummaryItem
    {
        public string EventTitle { get; set; }
        public int TicketsSold { get; set; }
        public int TotalCapacity { get; set; }
        public decimal AttendanceRate { get; set; } // Calculated as (TicketsSold / TotalCapacity) * 100
    }
}
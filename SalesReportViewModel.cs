// StarEvents/ViewModels/SalesReportViewModel.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace StarEvents.ViewModels
{
    public class SalesReportViewModel
    {
        public int Year { get; set; }
        public int Month { get; set; }

        [Display(Name = "Month")]
        public string MonthName => new DateTime(Year, Month, 1).ToString("MMMM yyyy");

        [Display(Name = "Total Revenue")]
        [DataType(DataType.Currency)]
        public decimal TotalRevenue { get; set; }

        [Display(Name = "Total Tickets Sold")]
        public int TotalTickets { get; set; }
    }
}
// StarEvents/ViewModels/EventsReportViewModel.cs

using System;
using System.ComponentModel.DataAnnotations;

namespace StarEvents.ViewModels
{
    public class EventsReportViewModel
    {
        public int EventId { get; set; }

        [Display(Name = "Event Title")] 
        public string Title { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Display(Name = "Tickets Sold")]
        public int TotalTicketsSold { get; set; } 

        [Display(Name = "Tickets Remaining")]
        public int AvailableTickets { get; set; }

        public string Status { get; set; }
    }
}
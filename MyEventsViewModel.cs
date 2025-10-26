// StarEvents.ViewModels/MyEventsViewModel.cs

using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering; // Required for SelectListItem

namespace StarEvents.ViewModels
{
    public class MyEventItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan Time { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal TicketPrice { get; set; }

        // Status can be "Pending", "Approved", "Rejected", etc.
        public string Status { get; set; }

        // These properties are required for the MyEvents table view
        public int TicketsSold { get; set; }
        public int TotalTickets { get; set; }
    }


    public class MyEventsViewModel
    {
        public List<MyEventItem> Events { get; set; } = new List<MyEventItem>();
        public string SearchTerm { get; set; }

        public string FilterStatus { get; set; } // Used for binding the selected value in the dropdown
        public int TotalEventsCount { get; set; }

        // *** FIX for RZ1031 Error in Razor View ***
        // This property provides the options for the Status dropdown using asp-items.
        public List<SelectListItem> StatusOptions { get; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "All", Text = "All Statuses" },
            new SelectListItem { Value = "Pending", Text = "Pending Review" },
            new SelectListItem { Value = "Approved", Text = "Approved (Live)" },
            new SelectListItem { Value = "Rejected", Text = "Rejected" },
            new SelectListItem { Value = "Cancelled", Text = "Cancelled" }
        };
    }
}
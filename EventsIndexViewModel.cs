using StarEvents.Models;

namespace StarEvents.ViewModels
{
    public class EventsIndexViewModel
    {
        public List<Event> Events { get; set; } = new List<Event>();
        public List<EventCategory> Categories { get; set; } = new List<EventCategory>();
        public List<string> Cities { get; set; } = new List<string>();
        public string SearchString { get; set; }
        public string SelectedCategory { get; set; }
        public string SelectedLocation { get; set; }
        public string SortOrder { get; set; }
    }
}
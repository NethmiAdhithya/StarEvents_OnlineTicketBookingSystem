namespace StarEvents.ViewModels
{
    public class AboutViewModel
    {
        public List<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
        public CompanyStats Stats { get; set; } = new CompanyStats();
    }

    public class TeamMember
    {
        public string Name { get; set; }
        public string Position { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public Dictionary<string, string> SocialLinks { get; set; } = new Dictionary<string, string>();
    }

    public class CompanyStats
    {
        public int EventsHosted { get; set; }
        public int HappyCustomers { get; set; }
        public int TicketsSold { get; set; }
        public int CitiesCovered { get; set; }
    }
}
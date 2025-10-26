using Microsoft.AspNetCore.Mvc;
using StarEvents.Data;
using Microsoft.EntityFrameworkCore;
using StarEvents.Models;
using StarEvents.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace StarEvents.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            // Get featured events for the homepage
            var featuredEvents = await _context.Events
                .Include(e => e.Venue)
                .Include(e => e.Category)
                .Where(e => e.EventDate >= DateTime.Today)
                .OrderBy(e => e.EventDate)
                .Take(3)
                .ToListAsync();

            return View(featuredEvents);
        }

        public IActionResult About()
        {
            var teamMembers = new List<TeamMember>
            {
                new TeamMember
                {
                    Name = "Nethmi Adhithya",
                    Position = "CEO & Founder",
                    ImageUrl = "/Images/ceo.jpg",
                    Description = "Visionary leader with 10+ years in event management industry.",
                    SocialLinks = new Dictionary<string, string>
                    {
                        { "linkedin", "https://linkedin.com/in/johnsmith" },
                        { "twitter", "https://twitter.com/johnsmith" }
                    }
                },
                new TeamMember
                {
                    Name = "Himashi Imanshika",
                    Position = "CTO",
                    ImageUrl = "/Images/cto.jpg",
                    Description = "Tech enthusiast building scalable solutions for event industry.",
                    SocialLinks = new Dictionary<string, string>
                    {
                        { "linkedin", "https://linkedin.com/in/sarahj" },
                        { "github", "https://github.com/sarahj" }
                    }
                },
                new TeamMember
                {
                    Name = "Dhanushi Wicramasinghe",
                    Position = "Head of Operations",
                    ImageUrl = "/Images/operations.jpg",
                    Description = "Ensuring seamless event experiences for all our customers.",
                    SocialLinks = new Dictionary<string, string>
                    {
                        { "linkedin", "https://linkedin.com/in/mikechen" }
                    }
                },
                new TeamMember
                {
                    Name = "Dihasha Minduli",
                    Position = "Customer Success Manager",
                    ImageUrl = "/Images/customer-success.jpg",
                    Description = "Dedicated to providing exceptional support to our users.",
                    SocialLinks = new Dictionary<string, string>
                    {
                        { "linkedin", "https://linkedin.com/in/emilydavis" },
                        { "twitter", "https://twitter.com/emilyd" }
                    }
                }
            };

            var stats = new CompanyStats
            {
                EventsHosted = 1250,
                HappyCustomers = 45000,
                TicketsSold = 120000,
                CitiesCovered = 25
            };

            var viewModel = new AboutViewModel
            {
                TeamMembers = teamMembers,
                Stats = stats
            };

            return View(viewModel);
        }

        // GET: /Home/Contact
        [HttpGet]
        public IActionResult Contact()
        {
            var contactInfo = new ContactInfo
            {
                Phone = "+94 11 234 5678",
                Email = "info@starevents.lk",
                Address = "123 Galle Road, Colombo 03, Sri Lanka",
                BusinessHours = "Monday - Friday: 9:00 AM - 6:00 PM\nSaturday: 10:00 AM - 4:00 PM\nSunday: Closed"
            };

            var viewModel = new ContactViewModel
            {
                ContactInfo = contactInfo,
                ContactForm = new ContactForm()
            };

            return View(viewModel);
        }

        // POST: /Home/Contact
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactForm contactForm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Here you would typically:
                    // 1. Save to database
                    // 2. Send email notification
                    // 3. Send auto-response to user

                    // Log the contact form submission
                    _logger.LogInformation("Contact form submitted: {Name} - {Email} - {Subject}",
                        contactForm.Name, contactForm.Email, contactForm.Subject);

                    // For now, we'll just show a success message
                    TempData["SuccessMessage"] = "Thank you for your message! We'll get back to you within 24 hours.";

                    // Clear the form
                    ModelState.Clear();

                    var contactInfo = new ContactInfo
                    {
                        Phone = "+94 11 234 5678",
                        Email = "info@starevents.lk",
                        Address = "123 Galle Road, Colombo 03, Sri Lanka",
                        BusinessHours = "Monday - Friday: 9:00 AM - 6:00 PM\nSaturday: 10:00 AM - 4:00 PM\nSunday: Closed"
                    };

                    var viewModel = new ContactViewModel
                    {
                        ContactInfo = contactInfo,
                        ContactForm = new ContactForm()
                    };

                    return View(viewModel);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing contact form");
                    ModelState.AddModelError("", "Sorry, there was an error sending your message. Please try again.");
                }
            }

            // If we got this far, something failed; redisplay form
            var contactInfoWithError = new ContactInfo
            {
                Phone = "+94 11 234 5678",
                Email = "info@starevents.lk",
                Address = "123 Galle Road, Colombo 03, Sri Lanka",
                BusinessHours = "Monday - Friday: 9:00 AM - 6:00 PM\nSaturday: 10:00 AM - 4:00 PM\nSunday: Closed"
            };

            var viewModelWithError = new ContactViewModel
            {
                ContactInfo = contactInfoWithError,
                ContactForm = contactForm
            };

            return View(viewModelWithError);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new Models.ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
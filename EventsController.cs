using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarEvents.Data;
using StarEvents.Models;
using StarEvents.ViewModels;
using Microsoft.AspNetCore.Hosting; // Required for accessing wwwroot
using System.IO; // Required for file operations
using System.Threading.Tasks;

namespace StarEvents.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EventsController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment; // <-- NEW dependency

        public EventsController(
            ApplicationDbContext context,
            ILogger<EventsController> logger,
            IWebHostEnvironment webHostEnvironment) // <-- Injected here
        {
            _context = context;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment; // <-- Assigned here
        }

        // GET: /Events
        [HttpGet]
        public async Task<IActionResult> Index(string searchString, string category, string location, string sortOrder)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["PriceSortParm"] = sortOrder == "Price" ? "price_desc" : "Price";

            var events = _context.Events
                .Include(e => e.Category)
                .Include(e => e.Venue)
                .Include(e => e.Organizer)
                // Filter to show only 'Approved' events to the public
                .Where(e => e.EventDate >= DateTime.Today && e.AvailableTickets > 0 && e.Status == "Approved")
                .AsQueryable();

            // Search filter
            if (!string.IsNullOrEmpty(searchString))
            {
                events = events.Where(e => e.Title.Contains(searchString) ||
                                             e.Description.Contains(searchString));
            }

            // Category filter
            if (!string.IsNullOrEmpty(category))
            {
                events = events.Where(e => e.Category.CategoryName == category);
            }

            // Location filter
            if (!string.IsNullOrEmpty(location))
            {
                events = events.Where(e => e.Venue.City.Contains(location));
            }

            // Sorting
            switch (sortOrder)
            {
                case "name_desc":
                    events = events.OrderByDescending(e => e.Title);
                    break;
                case "Date":
                    events = events.OrderBy(e => e.EventDate);
                    break;
                case "date_desc":
                    events = events.OrderByDescending(e => e.EventDate);
                    break;
                case "Price":
                    events = events.OrderBy(e => e.TicketPrice);
                    break;
                case "price_desc":
                    events = events.OrderByDescending(e => e.TicketPrice);
                    break;
                default:
                    events = events.OrderBy(e => e.EventDate);
                    break;
            }

            var eventCategories = await _context.EventCategories.ToListAsync();
            var cities = await _context.Venues.Select(v => v.City).Distinct().ToListAsync();

            var viewModel = new EventsIndexViewModel // Assuming you have an EventsIndexViewModel
            {
                Events = await events.ToListAsync(),
                Categories = eventCategories,
                Cities = cities,
                SearchString = searchString,
                SelectedCategory = category,
                SelectedLocation = location,
                SortOrder = sortOrder
            };

            return View(viewModel);
        }

        // GET: /Events/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var eventItem = await _context.Events
                .Include(e => e.Category)
                .Include(e => e.Venue)
                .Include(e => e.Organizer)
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (eventItem == null || eventItem.Status != "Approved")
            {
                return NotFound();
            }

            return View(eventItem);
        }

        // GET: /Events/Create (Organizers only)
        [HttpGet]
        [Authorize(Roles = "Organizer,Admin")]
        public async Task<IActionResult> Create()
        {
            var viewModel = new CreateEventViewModel // Use the updated ViewModel
            {
                Categories = await _context.EventCategories.ToListAsync(),
                Venues = await _context.Venues.Where(v => v.IsActive).ToListAsync()
            };

            return View(viewModel);
        }

        // POST: /Events/Create
        [HttpPost]
        [Authorize(Roles = "Organizer,Admin")]
        [ValidateAntiForgeryToken]

        [RequestSizeLimit(52428800)]
        public async Task<IActionResult> Create(CreateEventViewModel model) // Use the updated ViewModel
        {
            if (ModelState.IsValid)
            {
                string? uniqueFileName = null;
                string? imagePath = null;

                // --- IMAGE UPLOAD LOGIC ---
                if (model.ImageFile != null)
                {
                    // 1. Define folder path (e.g., wwwroot/images/events)
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "events");

                    // Ensure the directory exists
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // 2. Create unique file name
                    uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImageFile.FileName);

                    // 3. Define full file path
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // 4. Save the file to the server
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(fileStream);
                    }

                    // 5. Set the relative path to store in the database
                    imagePath = $"/images/events/{uniqueFileName}";
                }
                // --------------------------

                var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

                var eventItem = new Event
                {
                    Title = model.Title,
                    Description = model.Description,
                    EventDate = model.EventDate,
                    EventTime = model.EventTime,
                    TicketPrice = model.TicketPrice,
                    TotalTickets = model.TotalTickets,
                    AvailableTickets = model.TotalTickets,
                    OrganizerId = currentUser.Id,
                    VenueId = model.VenueId,
                    CategoryId = model.CategoryId,
                    Status = "Pending",
                    ImagePath = imagePath // <-- Save the path here
                };

                _context.Events.Add(eventItem);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Event created: {EventTitle} by {Organizer}", eventItem.Title, currentUser.UserName);
                TempData["SuccessMessage"] = $"Event '{model.Title}' created successfully and is now **Pending Admin Review**.";

                return RedirectToAction("MyEvents", "Organizer"); // Assuming MyEvents action is in OrganizerController
            }

            // Reload dropdown data if validation fails
            model.Categories = await _context.EventCategories.ToListAsync();
            model.Venues = await _context.Venues.Where(v => v.IsActive).ToListAsync();

            return View(model);
        }
    }
}
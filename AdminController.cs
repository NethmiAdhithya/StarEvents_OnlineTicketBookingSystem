using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarEvents.Data;
using StarEvents.ViewModels;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using StarEvents.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
// ADDED for file handling
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace StarEvents.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        // ADDED: IWebHostEnvironment for file path access
        private readonly IWebHostEnvironment _hostEnvironment;

        // UPDATED Constructor
        public AdminController(ApplicationDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _hostEnvironment = hostEnvironment; // Initialized
        }

        // GET: /Admin/Index (Admin Dashboard)
        public async Task<IActionResult> Index()
        {
            var model = new AdminDashboardViewModel
            {
                TotalUsers = await _context.Users.CountAsync(),
                TotalEvents = await _context.Events.CountAsync(),
                TotalRevenue = await _context.Payments.Where(p => p.PaymentStatus == "Completed").SumAsync(p => p.Amount),
                ActiveEvents = await _context.Events.CountAsync(e => e.EventDate >= DateTime.Today && e.Status == "Approved")
            };

            return View(model);
        }

        // -------------------------------------------------------------------
        // EVENT MANAGEMENT ACTIONS
        // -------------------------------------------------------------------

        // GET: /Admin/ManageEvents?searchTerm=...&filterStatus=...
        public async Task<IActionResult> ManageEvents(string searchTerm, string filterStatus = "All")
        {
            // --- 1. DATA QUERY LOGIC (REAL DATABASE QUERY) ---
            var eventsQuery = _context.Events
                .Include(e => e.Organizer)
                .Include(e => e.Venue)
                .AsQueryable();

            // --- 2. FILTER LOGIC ---
            if (filterStatus != "All")
            {
                eventsQuery = eventsQuery.Where(e => e.Status == filterStatus);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                eventsQuery = eventsQuery.Where(e =>
                    e.Title.Contains(searchTerm) ||
                    e.Organizer.UserName.Contains(searchTerm));
            }

            // Order by date, then status (Pending first)
            eventsQuery = eventsQuery
                .OrderBy(e => e.EventDate)
                .OrderBy(e => e.Status == "Pending" ? 0 : e.Status == "Approved" ? 1 : 2);


            // --- 3. MAP DATA TO VIEW MODEL ---
            var eventItems = await eventsQuery.Select(e => new EventManagementItem
            {
                Id = e.EventId,
                Title = e.Title,
                OrganizerName = e.Organizer.UserName,
                Date = e.EventDate,
                Time = e.EventTime,
                Location = e.Venue.VenueName + " (" + e.Venue.City + ")",
                TicketPrice = e.TicketPrice,
                Status = e.Status,

                ImagePath = e.ImagePath,

                TotalTickets = e.TotalTickets,
                TicketsSold = e.TotalTickets - e.AvailableTickets
            }).ToListAsync();


            // --- 4. BUILD VIEW MODEL AND RETURN ---
            var model = new ManageEventsViewModel
            {
                Events = eventItems,
                SearchTerm = searchTerm,
                FilterStatus = filterStatus
            };

            return View(model);
        }

        // GET: /Admin/EditEvent/5 (Display Edit Form)
        [HttpGet]
        public async Task<IActionResult> EditEvent(int id)
        {
            var eventItem = await _context.Events
                .Include(e => e.Venue)
                .Include(e => e.Category)
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (eventItem == null)
            {
                return NotFound();
            }

            // Map Event model to EditEventViewModel
            var viewModel = new EditEventViewModel
            {
                EventId = eventItem.EventId,
                Title = eventItem.Title,
                Description = eventItem.Description,
                EventDate = eventItem.EventDate,
                EventTime = eventItem.EventTime,
                TicketPrice = eventItem.TicketPrice,
                TotalTickets = eventItem.TotalTickets,
                VenueId = eventItem.VenueId,
                CategoryId = eventItem.CategoryId,

                // ADDED: Load existing image path
                ExistingImagePath = eventItem.ImagePath,

                // Populate dropdowns
                Categories = await _context.EventCategories.Where(ec => ec.IsActive).ToListAsync(),
                Venues = await _context.Venues.Where(v => v.IsActive).ToListAsync()
            };

            return View(viewModel);
        }

        // POST: /Admin/EditEvent/5 (Save Changes)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEvent(EditEventViewModel model)
        {
            // Get the event item from the database, but don't track the changes yet
            var eventItem = await _context.Events.AsNoTracking().FirstOrDefaultAsync(e => e.EventId == model.EventId);

            // Reload dropdown data, and existing image if validation fails
            if (!ModelState.IsValid)
            {
                model.Categories = await _context.EventCategories.Where(ec => ec.IsActive).ToListAsync();
                model.Venues = await _context.Venues.Where(v => v.IsActive).ToListAsync();

                // Retain existing image path on error (since it's not bound by IFormFile)
                if (eventItem != null)
                {
                    model.ExistingImagePath = eventItem.ImagePath;
                }
                return View(model);
            }

            // Now, track the entity again for updates
            eventItem = await _context.Events.FindAsync(model.EventId);

            if (eventItem == null)
            {
                return NotFound();
            }

            // --- IMAGE FILE HANDLING LOGIC ---
            if (model.NewImageFile != null)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string uploadPath = Path.Combine(wwwRootPath, "images", "events");

                // Ensure the directory exists
                Directory.CreateDirectory(uploadPath);

                // 1. Delete old image if it exists
                if (!string.IsNullOrEmpty(eventItem.ImagePath))
                {
                    // Remove leading '/'
                    string oldFilePath = Path.Combine(wwwRootPath, eventItem.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // 2. Save new file
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.NewImageFile.FileName);
                string filePath = Path.Combine(uploadPath, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.NewImageFile.CopyToAsync(fileStream);
                }

                // 3. Update ImagePath in Event model
                eventItem.ImagePath = $"/images/events/{fileName}";
            }
            // --- END IMAGE FILE HANDLING LOGIC ---


            // --- Update Core Properties ---
            eventItem.Title = model.Title;
            eventItem.Description = model.Description;
            eventItem.EventDate = model.EventDate;
            eventItem.EventTime = model.EventTime;
            eventItem.TicketPrice = model.TicketPrice;
            eventItem.VenueId = model.VenueId;
            eventItem.CategoryId = model.CategoryId;

            // --- Ticket Logic (Crucial for preventing over-selling) ---
            if (model.TotalTickets != eventItem.TotalTickets)
            {
                int ticketsSold = eventItem.TotalTickets - eventItem.AvailableTickets;

                if (model.TotalTickets < ticketsSold)
                {
                    // Error: New total is less than tickets already sold
                    ModelState.AddModelError("TotalTickets", $"New total tickets ({model.TotalTickets}) cannot be less than the {ticketsSold} tickets already sold.");
                    model.Categories = await _context.EventCategories.Where(ec => ec.IsActive).ToListAsync();
                    model.Venues = await _context.Venues.Where(v => v.IsActive).ToListAsync();

                    // Crucial: Pass the current ImagePath back on error
                    model.ExistingImagePath = eventItem.ImagePath;
                    return View(model);
                }

                // Calculate new available tickets
                eventItem.AvailableTickets = model.TotalTickets - ticketsSold;
                eventItem.TotalTickets = model.TotalTickets;
            }

            try
            {
                _context.Update(eventItem);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Event '{eventItem.Title}' updated successfully by Admin.";
                return RedirectToAction(nameof(ManageEvents));
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelState.AddModelError("", "A concurrency error occurred. The event may have been updated by another user.");
                model.Categories = await _context.EventCategories.Where(ec => ec.IsActive).ToListAsync();
                model.Venues = await _context.Venues.Where(v => v.IsActive).ToListAsync();

                // Crucial: Pass the current ImagePath back on error
                model.ExistingImagePath = eventItem.ImagePath;
                return View(model);
            }
        }

        // Action to approve an event
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveEvent(int id)
        {
            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem == null)
            {
                return NotFound();
            }

            // Update status
            eventItem.Status = "Approved";
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Event '{eventItem.Title}' has been successfully approved.";
            return RedirectToAction(nameof(ManageEvents));
        }

        // Action to reject an event
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectEvent(int id)
        {
            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem == null)
            {
                return NotFound();
            }

            // Update status
            eventItem.Status = "Rejected";
            await _context.SaveChangesAsync();

            TempData["WarningMessage"] = $"Event '{eventItem.Title}' has been rejected.";
            return RedirectToAction(nameof(ManageEvents));
        }

        // -------------------------------------------------------------------
        // USER MANAGEMENT ACTIONS
        // -------------------------------------------------------------------

        // GET: /Admin/ManageUsers?searchTerm=... (User List with Search and Roles)
        public async Task<IActionResult> ManageUsers(string searchTerm)
        {
            var usersQuery = _context.Users.AsNoTracking();

            // Search Logic
            if (!string.IsNullOrEmpty(searchTerm))
            {
                usersQuery = usersQuery.Where(u =>
                    u.UserName.Contains(searchTerm) ||
                    u.Email.Contains(searchTerm)
                );
            }

            var users = await usersQuery.ToListAsync();
            var userListItems = new List<UserListItemViewModel>();

            // Fetch role for each user asynchronously
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                userListItems.Add(new UserListItemViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Role = roles.FirstOrDefault() ?? "No Role"
                });
            }

            var model = new ManageUsersViewModel
            {
                Users = userListItems,
                SearchTerm = searchTerm
            };

            return View(model);
        }

        // GET: /Admin/AddUser (Show Add User Form with Roles)
        [HttpGet]
        public IActionResult AddUser()
        {
            ViewBag.Roles = _roleManager.Roles
                .Select(r => new SelectListItem { Value = r.Name, Text = r.Name })
                .ToList();

            return View();
        }

        // POST: /Admin/AddUser (Process Add User Form and Assign Role)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUser(AddUserViewModel model)
        {
            // Ensure roles are repopulated on failure
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = _roleManager.Roles
                    .Select(r => new SelectListItem { Value = r.Name, Text = r.Name })
                    .ToList();
                return View(model);
            }

            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.UserName,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(model.SelectedRole))
                {
                    await _userManager.AddToRoleAsync(user, model.SelectedRole);
                }

                return RedirectToAction(nameof(ManageUsers));
            }

            // Handle Errors (and repopulate roles on failure)
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            ViewBag.Roles = _roleManager.Roles
                .Select(r => new SelectListItem { Value = r.Name, Text = r.Name })
                .ToList();

            return View(model);
        }


        // GET: /Admin/MyProfile
        [HttpGet]
        public async Task<IActionResult> MyProfile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                // Safety redirect if user somehow gets here unauthenticated
                return RedirectToAction("Login", "Account");
            }

            ViewData["Title"] = "Administrator Profile";
            // Pass the User object to the view
            return View(user);
        }

        // GET: /Admin/EditProfile
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found or session expired.";
                return RedirectToAction("Login", "Account");
            }

            ViewData["Title"] = "Edit Administrator Profile";
            // Pass the full User model to the EditProfile view
            return View(user);
        }

        // POST: /Admin/EditProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(User model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Title"] = "Edit Administrator Profile";
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found. Please log in again.";
                return RedirectToAction("Login", "Account");
            }

            // Update only the editable fields (FirstName, LastName, PhoneNumber)
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;

            // Note: Email changes usually require a separate confirmation process.

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Your profile details have been successfully updated!";
                return RedirectToAction("MyProfile");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                ViewData["Title"] = "Edit Administrator Profile";
                return View(model);
            }
        }




        // GET: /Admin/Details/5 (User Details Page)
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // 1. Fetch the user by ID
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                TempData["WarningMessage"] = "User not found.";
                return RedirectToAction(nameof(ManageUsers));
            }

            // 2. Fetch the user's current role
            var roles = await _userManager.GetRolesAsync(user);

            // 3. Map to a ViewModel for display
            var viewModel = new UserDetailsViewModel // Assuming you have this ViewModel or use the User model
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = roles.FirstOrDefault() ?? "No Role"
            };

            ViewData["Title"] = $"Details for {user.UserName}";
            return View(viewModel);
        }



        // GET: /Admin/Edit/5 (Display Edit User Form)
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction(nameof(ManageUsers));
            }

            // 1. Get the current role(s)
            var userRoles = await _userManager.GetRolesAsync(user);
            string currentRole = userRoles.FirstOrDefault();

            // 2. Prepare the ViewModel
            var model = new EditUserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                CurrentRole = currentRole, // Store the user's current role
            };

            // 3. Populate all available roles for the dropdown
            ViewBag.Roles = _roleManager.Roles
                .Select(r => new SelectListItem { Value = r.Name, Text = r.Name, Selected = r.Name == currentRole })
                .ToList();

            ViewData["Title"] = $"Edit User: {user.UserName}";
            return View(model);
        }

        // POST: /Admin/Edit/5 (Process Edit User Form)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            // 1. Repopulate roles immediately in case of validation failure
            var rolesList = _roleManager.Roles
                .Select(r => new SelectListItem { Value = r.Name, Text = r.Name, Selected = r.Name == model.SelectedRole })
                .ToList();
            ViewBag.Roles = rolesList;

            if (!ModelState.IsValid)
            {
                ViewData["Title"] = $"Edit User: {model.UserName}";
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found. Could not save changes.";
                return RedirectToAction(nameof(ManageUsers));
            }

            // 2. Update core user details
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.UserName = model.UserName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                // 3. Handle Role Change
                if (model.SelectedRole != model.CurrentRole)
                {
                    // Remove all existing roles
                    var existingRoles = await _userManager.GetRolesAsync(user);
                    await _userManager.RemoveFromRolesAsync(user, existingRoles);

                    // Add the new selected role
                    if (!string.IsNullOrEmpty(model.SelectedRole))
                    {
                        await _userManager.AddToRoleAsync(user, model.SelectedRole);
                    }
                }

                TempData["SuccessMessage"] = $"User '{user.UserName}' details and role updated successfully.";
                return RedirectToAction(nameof(ManageUsers));
            }

            // 4. Handle Errors
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            ViewData["Title"] = $"Edit User: {model.UserName}";
            return View(model);
        }



        // GET: /Admin/Delete/5 (Display Delete Confirmation)
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                TempData["WarningMessage"] = "User not found. Cannot proceed with deletion.";
                return RedirectToAction(nameof(ManageUsers));
            }

            // We use the same UserDetailsViewModel for displaying information on the confirmation page
            var roles = await _userManager.GetRolesAsync(user);

            var viewModel = new UserDetailsViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = roles.FirstOrDefault() ?? "No Role"
            };

            ViewData["Title"] = $"Delete User: {user.UserName}";
            return View(viewModel);
        }

        // POST: /Admin/Delete/5 (Execute Deletion)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                TempData["WarningMessage"] = "User not found or already deleted.";
                return RedirectToAction(nameof(ManageUsers));
            }

            // IMPORTANT: Prevent Admin from deleting themselves.
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser.Id == id)
            {
                TempData["ErrorMessage"] = "You cannot delete your own Administrator account.";
                return RedirectToAction(nameof(ManageUsers));
            }

            // Execute the deletion
            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"User '{user.UserName}' has been successfully deleted.";
                return RedirectToAction(nameof(ManageUsers));
            }
            else
            {
                // Handle potential errors (e.g., database constraints)
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                TempData["ErrorMessage"] = "Error deleting user. See details for log.";
                return RedirectToAction(nameof(ManageUsers));
            }
        }


        [HttpGet]
        public IActionResult Reports()
        {
            // Simply returns the main Reports view where the admin can select a report type
            return View();
        }

        // You will also need separate actions for each report:
        [HttpGet]
        public async Task<IActionResult> SalesReport()
        {
            // Logic to fetch sales data (e.g., total revenue per month)
            var salesData = await _context.Bookings
                .GroupBy(b => new { b.BookingDate.Year, b.BookingDate.Month })
                .Select(g => new SalesReportViewModel
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalRevenue = g.Sum(b => b.TotalAmount),
                    TotalTickets = g.Sum(b => b.TicketQuantity)
                })
                .OrderByDescending(r => r.Year)
                .ThenByDescending(r => r.Month)
                .ToListAsync();

            return View(salesData);
        }

        [HttpGet]
        public async Task<IActionResult> UsersReport()
        {
            // Logic to fetch user data (e.g., user counts by role, sign-up dates)
            var users = await _userManager.Users.ToListAsync();
            var report = new List<UsersReportViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                report.Add(new UsersReportViewModel
                {
                    FullName = user.FullName,
                    Email = user.Email,
                    RegistrationDate = user.DateJoined, // <--- CORRECTED: Use DateJoined
                    Role = roles.FirstOrDefault() ?? "Customer"
                });
            }

            return View(report.OrderByDescending(u => u.RegistrationDate));
        }

        [HttpGet]
        public async Task<IActionResult> EventsReport()
        {
            // Logic to fetch event performance data (e.g., tickets sold, event status)
            var eventReports = await _context.Events
                .Select(e => new EventsReportViewModel
                {
                    EventId = e.EventId,
                    Title = e.Title,
                    Date = e.EventDate,
                    TotalTicketsSold = _context.Bookings.Where(b => b.EventId == e.EventId).Sum(b => (int?)b.TicketQuantity) ?? 0,
                    AvailableTickets = e.AvailableTickets,
                    Status = e.Status
                })
                .OrderByDescending(e => e.Date)
                .ToListAsync();

            return View(eventReports);
        }


    }
}
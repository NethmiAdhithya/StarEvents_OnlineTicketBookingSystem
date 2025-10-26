using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using StarEvents.Models;
using StarEvents.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;

namespace StarEvents.Controllers
{
    // Ensure only logged-in users (Customers) can access the dashboard
    [Authorize(Roles = "Customer")]
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public CustomerController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Customer/Dashboard
        public IActionResult Dashboard()
        {
            return View(); // Renders Views/Customer/Dashboard.cshtml
        }

        // GET: /Customer/MyTickets (Section 3.4 E-Ticket Management)
        public async Task<IActionResult> MyTickets()
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // ⭐ MODIFIED QUERY: Temporarily remove BookingStatus filter to see ALL bookings ⭐
            var bookings = await _context.Bookings
                .Where(b => b.UserId == userId) // Keep the user filter
                .Include(b => b.Event)
                    .ThenInclude(e => e.Venue)
                .Include(b => b.Tickets)
                .Include(b => b.Payment)
                .OrderBy(b => b.Event.EventDate)
                .ToListAsync();

            return View(bookings);
        }

        [HttpGet]
        public async Task<IActionResult> BookingHistory()
        {
            // 1. Get the ID of the currently logged-in user
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // 2. Fetch all bookings for this user (past and future, any status)
            var bookings = await _context.Bookings
                .Where(b => b.UserId == userId)
                .Include(b => b.Event)
                    .ThenInclude(e => e.Venue) // Assuming Event has a Venue
                .Include(b => b.Payment) // Include the singular Payment object
                .Include(b => b.Tickets) // Include tickets if you want details
                                         // Order by date descending to show recent history first
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            // 3. Pass the list of bookings (which is IEnumerable<Booking>) to the view
            return View(bookings);
        }

        // GET: /Customer/MyProfile
        public async Task<IActionResult> MyProfile()
        {
            // Use the UserManager to get the fully populated User object for the logged-in user
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                // Should not happen if [Authorize] is used, but it's a good safety check
                return RedirectToAction("Login", "Account");
            }

            ViewData["Title"] = "My Profile";
            // Pass the User object (which is your StarEvents.Models.User) to the view
            return View(user);
        }

        // GET: /Customer/EditProfile
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found or session expired.";
                return RedirectToAction("Login", "Account");
            }

            // Pass the full User model to the EditProfile view
            ViewData["Title"] = "Edit Profile";
            return View(user);
        }

        // POST: /Customer/EditProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(User model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Title"] = "Edit Profile";
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found. Please log in again.";
                return RedirectToAction("Login", "Account");
            }

            user.FirstName = model.FirstName; // Update the editable source property
            user.LastName = model.LastName;   // Update the editable source property

            // user.Email = model.Email; // Keep as read-only or separate process
            user.PhoneNumber = model.PhoneNumber;

            // NOTE: You must include all properties you want the user to edit in the model and form.
            // For example, if you have a custom 'Address' property, you'd update it here:
            // user.Address = model.Address;

            // Use UserManager to update the user in the database
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                // Re-sign in the user to refresh their claims if any were changed (like username/email)
                await _userManager.UpdateSecurityStampAsync(user);

                TempData["SuccessMessage"] = "Your profile details have been successfully updated!";
                return RedirectToAction("MyProfile");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                ViewData["Title"] = "Edit Profile";
                return View(model);
            }
        }

        // GET: /Customer/LoyaltyPoints
        public IActionResult LoyaltyPoints()
        {
            ViewData["Title"] = "Loyalty Points";
            return View();
        }

        // GET: /Customer/Notifications
        public IActionResult Notifications()
        {
            ViewData["Title"] = "Notifications";
            return View();
        }


        // GET: /Customer/DownloadTicketPDF/{ticketId}
        [HttpGet]
        public async Task<IActionResult> DownloadTicketPDF(int ticketId)
        {
            var userId = _userManager.GetUserId(User);

            // 1. Fetch the specific ticket, ensuring it belongs to the current user
            var ticket = await _context.Tickets
                .Include(t => t.Booking)
                    .ThenInclude(b => b.Event)
                        .ThenInclude(e => e.Venue)
                .FirstOrDefaultAsync(t => t.TicketId == ticketId && t.Booking.UserId == userId);

            if (ticket == null)
            {
                TempData["ErrorMessage"] = "Ticket not found or you do not have permission to download it.";
                return RedirectToAction("MyTickets");
            }

            // 2. --- PDF GENERATION LOGIC ---
            // NOTE: Real-world PDF generation requires a library (e.g., iTextSharp, QuestPDF, or HTML-to-PDF service).
            // This is a simplified simulation based on the ticket data.

            var pdfContent = $"StarEvents E-Ticket\n" +
                             $"--------------------------\n" +
                             $"Event: {ticket.Booking.Event.Title}\n" +
                             $"Venue: {ticket.Booking.Event.Venue.VenueName}\n" +
                             $"Date: {ticket.Booking.Event.EventDate.ToString("dd MMM yyyy")} at {ticket.Booking.Event.EventTime.ToString("hh\\:mm tt")}\n" +
                             $"Ticket Number: {ticket.TicketNumber}\n" +
                             $"Booking Reference: {ticket.Booking.BookingReference}\n" +
                             $"Attendee: {ticket.AttendeeName}\n" +
                             $"Status: {ticket.Status}\n" +
                             $"QR Code Data (Base64): {ticket.QRCode.Substring(0, 50)}...\n"; // Only show snippet

            // For demonstration, we will return a simple text file with .pdf extension,
            // or you could return a byte array from a real PDF generation library.

            // In a real application, you would replace this with:
            // byte[] pdfBytes = new PdfGeneratorService().Generate(ticket); 
            // return File(pdfBytes, "application/pdf", $"{ticket.TicketNumber}_Ticket.pdf");

            // Simplified FileResult for a text file disguised as a PDF:
            var contentBytes = System.Text.Encoding.UTF8.GetBytes(pdfContent);

            return File(
                contentBytes,
                "application/pdf",
                $"{ticket.TicketNumber}_E-Ticket.pdf" // Filename for download
            );
        }
    }
}
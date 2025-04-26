using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TintApp.Data;

namespace TintApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Bookings()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetPendingBookings()
        {
            var pending = await _context.Bookings
                .Include(b => b.ServiceItem) // এইটা যোগ করতে হবে
                .Where(b => b.Status == "Pending")
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            return PartialView("_PendingBookingsPartial", pending);
        }

        [HttpGet]
        public async Task<IActionResult> GetCompletedBookings()
        {
            var completed = await _context.Bookings
                .Include(b => b.ServiceItem) // এইটাও লাগবে
                .Where(b => b.Status == "Done")
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            return PartialView("_CompletedBookingsPartial", completed);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsDone(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                booking.Status = "Done";
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}

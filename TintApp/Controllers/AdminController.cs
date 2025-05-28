using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TintApp.Data;

namespace TintApp.Controllers
{
    [Authorize(Roles = "Admin, SuperAdmin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult AdminDashboard()
        {
            return View();
        }

        public async Task<IActionResult> Bookings()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetPendingBookings(int page = 1, int pageSize = 5, string search = "", DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.Bookings
                .Include(b => b.ServiceItem)
                .Where(b => b.Status == "Pending");

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(b => b.CustomerName.Contains(search) || b.ServiceItem.ItemName.Contains(search));
            }

            if (fromDate.HasValue)
            {
                query = query.Where(b => b.BookingDate.Date >= fromDate.Value.Date);
            }
            if (toDate.HasValue)
            {
                query = query.Where(b => b.BookingDate.Date <= toDate.Value.Date);
            }

            var totalRecords = await query.CountAsync();
            var bookings = await query
                .OrderByDescending(b => b.BookingNumber)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();


            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            ViewBag.PageSize = pageSize;

            // Calculate the starting serial number for the current page
            int serialStart = (page - 1) * pageSize + 1;

            // Pass the bookings and serial number starting point to the view
            ViewBag.SerialStart = serialStart;

            return PartialView("_PendingBookingsPartial", bookings);
        }

        [HttpGet]
        public async Task<IActionResult> GetCompletedBookings(int page = 1, int pageSize = 5, string search = "", DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.Bookings
                .Include(b => b.ServiceItem)
                .Where(b => b.Status == "Done");

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(b => b.CustomerName.Contains(search) || b.ServiceItem.ItemName.Contains(search));
            }

            if (fromDate.HasValue)
            {
                query = query.Where(b => b.BookingDate.Date >= fromDate.Value.Date);
            }
            if (toDate.HasValue)
            {
                query = query.Where(b => b.BookingDate.Date <= toDate.Value.Date);
            }

            var totalRecords = await query.CountAsync();
            var bookings = await query
                .OrderByDescending(b => b.BookingDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            ViewBag.PageSize = pageSize;

            // Calculate the starting serial number for the current page
            int serialStart = (page - 1) * pageSize + 1;

            // Pass the bookings and serial number starting point to the view
            ViewBag.SerialStart = serialStart;

            return PartialView("_CompletedBookingsPartial", bookings);
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

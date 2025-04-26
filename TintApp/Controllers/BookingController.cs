using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using System.Reflection.Metadata;
using TintApp.Data;
using TintApp.Models;

namespace TintApp.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BookingController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        // GET: Booking/Create
        public IActionResult Create()
        {
            ViewBag.ServiceItems = _context.ServiceItems.ToList();
            ViewBag.ServiceCategories = new SelectList(_context.ServiceCategories, "Id", "CategoryName");
            return View();
        }

        // POST: Booking/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking booking)
        {
            if (ModelState.IsValid)
            {
                // 1. Set Status
                booking.Status = "Pending"; // Create সময় Status = Pending

                // 2. Load related ServiceItem to use in PDF
                booking.ServiceItem = await _context.ServiceItems.FindAsync(booking.ServiceItemId);

                // 3. Generate PDF and set PdfUrl
                var pdfUrl = GenerateBookingPdf(booking);
                booking.PdfUrl = pdfUrl;

                // 4. Finally save to database
                _context.Add(booking);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Booking successful! Download your booking receipt:";
                TempData["PdfUrl"] = pdfUrl;
                return RedirectToAction(nameof(Create));
            }

            TempData["ErrorMessage"] = "Please carefully fill up the form and try again.";
            ViewBag.ServiceCategories = new SelectList(_context.ServiceCategories, "Id", "CategoryName", booking.ServiceCategoryId);
            return View(booking);
        }

        // PDF Generation Method
        private string GenerateBookingPdf(Booking booking)
        {
            var webRootPath = _webHostEnvironment.WebRootPath;
            var pdfFolderPath = Path.Combine(webRootPath, "pdfs");

            if (!Directory.Exists(pdfFolderPath))
            {
                Directory.CreateDirectory(pdfFolderPath);
            }

            var filePath = Path.Combine(pdfFolderPath, $"{booking.BookingNumber}.pdf");

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                var document = new iTextSharp.text.Document();
                PdfWriter.GetInstance(document, stream);
                document.Open();

                document.Add(new Paragraph("Company Name: Tint ML"));
                document.Add(new Paragraph($"Booking Number: {booking.BookingNumber}"));
                document.Add(new Paragraph($"Customer Name: {booking.CustomerName}"));
                document.Add(new Paragraph($"Car Model: {booking.CarModel} - Plate: {booking.CarNumberPlate}"));
                document.Add(new Paragraph($"Booking Date: {booking.BookingDate.ToString("dd/MM/yyyy hh:mm tt")}"));
                document.Add(new Paragraph($"Service: {booking.ServiceItem?.ItemName ?? "N/A"}"));
                document.Add(new Paragraph($"Price: {booking.Price}"));
                document.Close();
            }

            return "/pdfs/" + $"{booking.BookingNumber}.pdf";
        }

        // GET: Booking/Success
        public IActionResult Success()
        {
            return View();
        }

        //public IActionResult GetServiceItems(int categoryId)
        //{
        //    var serviceItems = _context.ServiceItems
        //        .Where(s => s.ServiceCategoryId == categoryId)
        //        .ToList();

        //    return Json(serviceItems);
        //}
        public JsonResult GetServiceItems(int categoryId)
        {
            var items = _context.ServiceItems
                .Where(x => x.ServiceCategoryId == categoryId)
                .Select(x => new { x.Id, x.ItemName, x.Price })
                .ToList();
            return Json(items);
        }

        //[HttpGet]
        //public JsonResult GetServiceItemPrice(int serviceItemId)
        //{
        //    var item = _context.ServiceItems.FirstOrDefault(x => x.Id == serviceItemId);
        //    return Json(new { price = item?.Price ?? 0 });
        //}
    }
}

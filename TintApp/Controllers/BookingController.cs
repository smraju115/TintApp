using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using System.Reflection.Metadata;
using TintApp.Data;
using TintApp.Models;

namespace TintApp.Controllers
{
    [Authorize(Roles = "Admin, SuperAdmin")]
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

        //PDF Generation Method
        //private string GenerateBookingPdf(Booking booking)
        //{
        //    var webRootPath = _webHostEnvironment.WebRootPath;
        //    var pdfFolderPath = Path.Combine(webRootPath, "pdfs");

        //    if (!Directory.Exists(pdfFolderPath))
        //    {
        //        Directory.CreateDirectory(pdfFolderPath);
        //    }

        //    var filePath = Path.Combine(pdfFolderPath, $"{booking.BookingNumber}.pdf");

        //    using (var stream = new FileStream(filePath, FileMode.Create))
        //    {
        //        var document = new iTextSharp.text.Document();
        //        PdfWriter.GetInstance(document, stream);
        //        document.Open();

        //        document.Add(new Paragraph("Company Name: Tint ML"));
        //        document.Add(new Paragraph($"Booking Number: {booking.BookingNumber}"));
        //        document.Add(new Paragraph($"Customer Name: {booking.CustomerName}"));
        //        document.Add(new Paragraph($"Car Model: {booking.CarModel} - Plate: {booking.CarNumberPlate}"));
        //        document.Add(new Paragraph($"Booking Date: {booking.BookingDate.ToString("dd/MM/yyyy hh:mm tt")}"));
        //        document.Add(new Paragraph($"Service: {booking.ServiceItem?.ItemName ?? "N/A"}"));
        //        document.Add(new Paragraph($"Price: {booking.Price}"));
        //        document.Close();
        //    }

        //    return "/pdfs/" + $"{booking.BookingNumber}.pdf";
        //}


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
                var document = new iTextSharp.text.Document(PageSize.A4, 36, 36, 36, 36);
                var writer = PdfWriter.GetInstance(document, stream);
                document.Open();

                // Logo image
                string logoPath = Path.Combine(webRootPath, "images", "logo.png"); // তুমি logo.png ওয়েব root-এর images ফোল্ডারে রাখবা
                if (System.IO.File.Exists(logoPath))
                {
                    var logo = Image.GetInstance(logoPath);
                    logo.ScaleAbsolute(80f, 80f); // লোগোর সাইজ
                    logo.Alignment = Image.ALIGN_LEFT;
                    document.Add(logo);
                }

                // Company Name
                var companyName = new Paragraph("Tint ML", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 20f, iTextSharp.text.Font.BOLD));
                companyName.Alignment = Element.ALIGN_CENTER;
                document.Add(companyName);

                // Address
                var address = new Paragraph("123 Main Street, Dhaka, Bangladesh\nMobile: 017xxxxxxxx\nEmail: info@tintml.com", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12f));
                address.Alignment = Element.ALIGN_CENTER;
                document.Add(address);

                // Divider line
                document.Add(new Paragraph(" "));
                var line = new iTextSharp.text.pdf.draw.LineSeparator(1.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_CENTER, 1);
                document.Add(new Chunk(line));
                document.Add(new Paragraph(" "));

                // Booking Receipt Heading
                var heading = new Paragraph("Booking Receipt", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 16f, iTextSharp.text.Font.BOLD));
                heading.Alignment = Element.ALIGN_CENTER;
                document.Add(heading);
                document.Add(new Paragraph(" "));

                // Booking Details
                PdfPTable table = new PdfPTable(2);
                table.WidthPercentage = 100;
                table.SpacingBefore = 10f;
                table.SpacingAfter = 10f;
                float[] widths = { 1f, 2f };
                table.SetWidths(widths);

                table.AddCell("Booking Number:");
                table.AddCell(booking.BookingNumber);

                table.AddCell("Customer Name:");
                table.AddCell(booking.CustomerName);

                table.AddCell("Car Model & Plate:");
                table.AddCell($"{booking.CarModel} - {booking.CarNumberPlate}");

                table.AddCell("Booking Date:");
                table.AddCell(booking.BookingDate.ToString("dd/MM/yyyy hh:mm tt"));

                table.AddCell("Service:");
                table.AddCell(booking.ServiceItem?.ItemName ?? "N/A");

                table.AddCell("Price:");
                table.AddCell(booking.Price?.ToString("N2") + " BDT");

                document.Add(table);

                // Barcode
                var barcode128 = new Barcode128
                {
                    Code = booking.BookingNumber,
                    StartStopText = false,
                    CodeType = Barcode.CODE128
                };
                Image barcodeImage = barcode128.CreateImageWithBarcode(writer.DirectContent, null, null);
                barcodeImage.Alignment = Image.ALIGN_CENTER;
                document.Add(barcodeImage);

                document.Add(new Paragraph(" "));

                // Slogan
                var slogan = new Paragraph("\"Your Satisfaction is Our Priority!\"", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12f, iTextSharp.text.Font.ITALIC, BaseColor.GRAY));
                slogan.Alignment = Element.ALIGN_CENTER;
                document.Add(slogan);

                document.Add(new Paragraph(" "));

                // Signature Line
                var signLine = new Paragraph("Authorized Signature", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12f));
                signLine.Alignment = Element.ALIGN_RIGHT;
                document.Add(signLine);

                // Signature Image
                string signPath = Path.Combine(webRootPath, "images", "signature.png"); // তোমার সিগনেচার ইমেজ এখানে থাকবে
                if (System.IO.File.Exists(signPath))
                {
                    var signImage = Image.GetInstance(signPath);
                    signImage.ScaleAbsolute(120f, 40f); // Signature image size
                    signImage.Alignment = Image.ALIGN_RIGHT;
                    document.Add(signImage);
                }

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

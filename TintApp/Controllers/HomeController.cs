using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TintApp.Data;
using TintApp.Models;

namespace TintApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        

        public async Task<IActionResult> Index()
        {
            //// ViewBag এ ক্যাটেগরি পাঠানো
            //ViewBag.ServiceCategories = _context.ServiceCategories.ToList();

            //// সব আইটেম পাঠানো — Category অনুযায়ী JS দিয়ে filter করবো
            //ViewBag.ServiceItems = _context.ServiceItems.ToList();

            var categories = await _context.ServiceCategories.ToListAsync();
            return View(categories);
        }


        // Public View
        //[AllowAnonymous]
        //public IActionResult PublicDetails(int categoryId)
        //{
        //    var category = _context.ServiceCategories
        //        .Include(c => c.Items)
        //        .FirstOrDefault(c => c.Id == categoryId);

        //    if (category == null)
        //        return NotFound();

        //    return View("PublicDetails", category);
        //}



       
    }
}

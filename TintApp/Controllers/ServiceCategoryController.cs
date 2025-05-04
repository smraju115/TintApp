using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TintApp.Data;
using TintApp.Models;
using TintApp.ViewModels;

namespace TintApp.Controllers
{
    [Authorize(Roles = "Admin, SuperAdmin")]
    public class ServiceCategoryController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly ApplicationDbContext _context;

        public ServiceCategoryController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // Show all categories with item count
        public async Task<IActionResult> Index()
        {
            
            ViewBag.ServiceCategories = await _context.ServiceCategories
                .Where(c => !c.IsDeleted) 
                .ToListAsync();

            
            ViewBag.ServiceItems = await _context.ServiceItems
                .Where(i => !i.IsDeleted)
                .ToListAsync();

            
            var categories = await _context.ServiceCategories
                .Include(c => c.Items.Where(i => !i.IsDeleted)) // 
                .Where(c => !c.IsDeleted) // 
                .ToListAsync();

            return View(categories);
        }

        // GET: Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceCategory model, IFormFile ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null)
                {
                    string wwwRootPath = _env.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                    string path = Path.Combine(wwwRootPath, "uploads/category", fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }

                    model.ImageUrl = "/uploads/category/" + fileName;
                }

                _context.ServiceCategories.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        //Admin View Details ViewModel
        public async Task<IActionResult> Details(int id)
        {
            var category = await _context.ServiceCategories.FindAsync(id);
            var items = await _context.ServiceItems
                .Where(x => x.ServiceCategoryId == id && !x.IsDeleted) //  Soft-deleted ignore
                 .Include(i => i.Images) // for multiple images
                .ToListAsync();

            var vm = new CategoryWithItemsVM
            {
                Category = category,
                Items = items
            };

            return View(vm);
        }


        //Only for Public View
        [AllowAnonymous]
        public IActionResult PublicDetails(int categoryId)
        {

            var category = _context.ServiceCategories
                .Include(c => c.Items)
                .ThenInclude(i => i.Images)
                .FirstOrDefault(c => c.Id == categoryId);

            if (category == null)
                return NotFound();

            return View("PublicDetails", category);
        }


        // Delete category
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.ServiceCategories.FindAsync(id);
            if (category != null)
            {
                // Delete the image from wwwroot/uploads/category/
                if (!string.IsNullOrEmpty(category.ImageUrl))
                {
                    string wwwRootPath = _env.WebRootPath;
                    string fullPath = Path.Combine(wwwRootPath, category.ImageUrl.TrimStart('/')); // TrimStart('/') ensures no slash issue

                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath); // ✅ Delete the file physically
                    }
                }

                category.IsDeleted = true; // ✅ Soft delete
                _context.ServiceCategories.Update(category);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }






    }
}

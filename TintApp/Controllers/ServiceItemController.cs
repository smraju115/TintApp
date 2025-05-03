using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TintApp.Data;
using TintApp.Models;

namespace TintApp.Controllers
{
    [Authorize(Roles = "Admin, SuperAdmin")]
    public class ServiceItemController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ServiceItemController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        

        public IActionResult Create(int id)
        {
            ViewBag.CategoryId = id;
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(ServiceItem model, IFormFile PictureFile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.Id = 0; // Make sure EF can auto-generate the ID

                    if (PictureFile != null)
                    {
                        string wwwRootPath = _env.WebRootPath;
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(PictureFile.FileName);
                        string path = Path.Combine(wwwRootPath, "uploads/item", fileName);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await PictureFile.CopyToAsync(stream);
                        }

                        model.PictureUrl = "/uploads/item/" + fileName;
                    }

                    _context.ServiceItems.Add(model);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "ServiceCategory");
                }

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Something went wrong: " + ex.Message);
            }


            ViewBag.CategoryId = model.ServiceCategoryId;
            return View(model);
        }


        // Delete
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var item = await _context.ServiceItems.FindAsync(id);
        //    if (item != null)
        //    {
        //        _context.ServiceItems.Remove(item);
        //        await _context.SaveChangesAsync();
        //    }
        //    return RedirectToAction("Index", "ServiceCategory");
        //}


        //Item Delete Method
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.ServiceItems.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            // Delete image from folder if exists
            if (!string.IsNullOrEmpty(item.PictureUrl))
            {
                var imagePath = Path.Combine(_env.WebRootPath, item.PictureUrl.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            item.IsDeleted = true; // <-- Soft delete
            _context.ServiceItems.Update(item);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "ServiceCategory");
        }
    }
}

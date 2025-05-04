using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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


        //[HttpPost]
        //public async Task<IActionResult> Create(ServiceItem model, IFormFile PictureFile)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            model.Id = 0; // Make sure EF can auto-generate the ID

        //            if (PictureFile != null)
        //            {
        //                string wwwRootPath = _env.WebRootPath;
        //                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(PictureFile.FileName);
        //                string path = Path.Combine(wwwRootPath, "uploads/item", fileName);

        //                using (var stream = new FileStream(path, FileMode.Create))
        //                {
        //                    await PictureFile.CopyToAsync(stream);
        //                }

        //                model.PictureUrl = "/uploads/item/" + fileName;
        //            }

        //            _context.ServiceItems.Add(model);
        //            await _context.SaveChangesAsync();
        //            return RedirectToAction("Index", "ServiceCategory");
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        ModelState.AddModelError("", "Something went wrong: " + ex.Message);
        //    }


        //    ViewBag.CategoryId = model.ServiceCategoryId;
        //    return View(model);
        //}

        [HttpPost]
        public async Task<IActionResult> Create(ServiceItem model, List<IFormFile> PictureFiles)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.Id = 0;
                    _context.ServiceItems.Add(model);
                    await _context.SaveChangesAsync();

                    if (PictureFiles != null && PictureFiles.Count > 0)
                    {
                        foreach (var file in PictureFiles)
                        {
                            if (file.Length > 0)
                            {
                                string wwwRootPath = _env.WebRootPath;
                                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                                string path = Path.Combine(wwwRootPath, "uploads/item", fileName);

                                using (var stream = new FileStream(path, FileMode.Create))
                                {
                                    await file.CopyToAsync(stream);
                                }

                                var image = new ServiceImage
                                {
                                    ServiceItemId = model.Id,
                                    ImageUrl = "/uploads/item/" + fileName
                                };

                                _context.ServiceImages.Add(image);
                            }
                        }
                        await _context.SaveChangesAsync();
                    }

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





        //Service Item Delete Method
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var item = await _context.ServiceItems.FindAsync(id);
        //    if (item == null)
        //    {
        //        return NotFound();
        //    }

        //    // Delete image from folder if exists
        //    if (!string.IsNullOrEmpty(item.PictureUrl))
        //    {
        //        var imagePath = Path.Combine(_env.WebRootPath, item.PictureUrl.TrimStart('/'));
        //        if (System.IO.File.Exists(imagePath))
        //        {
        //            System.IO.File.Delete(imagePath);
        //        }
        //    }

        //    item.IsDeleted = true; // <-- Soft delete
        //    _context.ServiceItems.Update(item);
        //    await _context.SaveChangesAsync();

        //    return RedirectToAction("Index", "ServiceCategory");
        //}

        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.ServiceItems
                .Include(x => x.Images)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            // Delete images from folder
            if (item.Images != null)
            {
                foreach (var image in item.Images)
                {
                    var imagePath = Path.Combine(_env.WebRootPath, image.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _context.ServiceImages.RemoveRange(item.Images);
            }

            item.IsDeleted = true;
            _context.ServiceItems.Update(item);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "ServiceCategory");
        }


    }
}

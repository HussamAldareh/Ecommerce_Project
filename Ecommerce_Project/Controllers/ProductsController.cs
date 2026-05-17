using Ecommerce_Project.Data;
using Ecommerce_Project.Helpers;
using Ecommerce_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce_Project.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index(string search, decimal? minPrice, decimal? maxPrice)
        {
            var productsQuery = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                productsQuery = productsQuery.Where(p => p.Name.Contains(search));
            }

            if (minPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price <= maxPrice.Value);
            }

            var finalResult = productsQuery
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.Discounts)
                .Include(p => p.WishlistItems);

            return View(await finalResult.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.Discounts)
                 .Include(p => p.ProductComments)
                    .ThenInclude(c => c.User)
                    .Include(p => p.ProductRatings)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null) return NotFound();

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, List<IFormFile> imageFiles)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();

                if (imageFiles != null && imageFiles.Count > 0)
                {
                    foreach (var file in imageFiles)
                    {
                        var relativePath = await FileHelper.UploadFile(file, "images/products");

                        if (!string.IsNullOrEmpty(relativePath))
                        {

                            _context.ProductsImages.Add(new ProductImage
                            {
                                ProductId = product.Id,
                                ImageUrl = relativePath
                            });
                        }
                    }
                    await _context.SaveChangesAsync();
                }
                TempData["Notification"] = $"{product.Name} has been added successfully.";
                TempData["NotificationType"] = "success";

                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null) return NotFound();

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,StockQuantity,CategoryId")] Product product, List<IFormFile> newImages)
        {
            if (id != product.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();

                    if (newImages != null && newImages.Count > 0)
                    {
                        foreach (var file in newImages)
                        {
                            var relativePath = await FileHelper.UploadFile(file, "images/products");
                            if (!string.IsNullOrEmpty(relativePath))
                            {
                                _context.ProductsImages.Add(new ProductImage
                                {
                                    ProductId = product.Id,
                                    ImageUrl = relativePath
                                });
                            }
                        }
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id)) return NotFound();
                    else throw;
                }
                TempData["Notification"] = $"Changes to {product.Name} have been saved.";
                TempData["NotificationType"] = "success";

                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Products/DeleteImage (AJAX)
        [HttpPost]
        public async Task<IActionResult> DeleteImage(int id)
        {

            var img = await _context.ProductsImages.FindAsync(id);
            if (img == null) return NotFound();

            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", img.ImageUrl.TrimStart('/'));
            if (System.IO.File.Exists(fullPath)) System.IO.File.Delete(fullPath);

            _context.ProductsImages.Remove(img);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null) return NotFound();

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product != null)
            {
                foreach (var img in product.ProductImages)
                {
                    var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", img.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(fullPath)) System.IO.File.Delete(fullPath);
                }

                _context.Products.Remove(product);
            }
            TempData["Notification"] = $"{product.Name} has been permanently deleted.";
            TempData["NotificationType"] = "warning";

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        public async Task<IActionResult> ByCategory(int id)
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Where(p => p.CategoryId == id)
                .ToListAsync();

            return View("Index", products);
        }



        /*
        public async Task<IActionResult> Search(string query)
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Where(p => p.Name.Contains(query) || p.Description.Contains(query))
                .ToListAsync();
            return View("Index", products);
        }
        
        */

        public async Task<IActionResult> Offers()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.Discounts)
                .Where(p => p.Discounts.Any(d =>
                    d.IsActive &&
                    d.StartDate <= DateTime.Now &&
                    d.EndDate >= DateTime.Now))
                .ToListAsync();

            return View("Index", products);
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
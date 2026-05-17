using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ecommerce_Project.Data;
using Ecommerce_Project.Models;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce_Project.Controllers
{
    public class ProductRatingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public ProductRatingsController(
      ApplicationDbContext context,
      UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        // GET: ProductRatings
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.productRatings.Include(p => p.Product).Include(p => p.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ProductRatings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productRating = await _context.productRatings
                .Include(p => p.Product)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productRating == null)
            {
                return NotFound();
            }

            return View(productRating);
        }

        // GET: ProductRatings/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: ProductRatings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        
        public async Task<IActionResult> Create(int productId, int ratingValue)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var existingRating = await _context.productRatings
                .FirstOrDefaultAsync(r =>
                    r.ProductId == productId &&
                    r.UserId == user.Id);

            if (existingRating != null)
            {
                existingRating.RatingValue = ratingValue;
            }
            else
            {
                ProductRating rating = new ProductRating
                {
                    ProductId = productId,
                    RatingValue = ratingValue,
                    UserId = user.Id,
                    IsApproved = true
                };

                _context.productRatings.Add(rating);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Products", new { id = productId });
        }

        // GET: ProductRatings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productRating = await _context.productRatings.FindAsync(id);
            if (productRating == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", productRating.ProductId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", productRating.UserId);
            return View(productRating);
        }

        // POST: ProductRatings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RatingValue,IsApproved,UserId,ProductId")] ProductRating productRating)
        {
            if (id != productRating.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productRating);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductRatingExists(productRating.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", productRating.ProductId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", productRating.UserId);
            return View(productRating);
        }

        // GET: ProductRatings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productRating = await _context.productRatings
                .Include(p => p.Product)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productRating == null)
            {
                return NotFound();
            }

            return View(productRating);
        }

        // POST: ProductRatings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productRating = await _context.productRatings.FindAsync(id);
            if (productRating != null)
            {
                _context.productRatings.Remove(productRating);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductRatingExists(int id)
        {
            return _context.productRatings.Any(e => e.Id == id);
        }
    }
}

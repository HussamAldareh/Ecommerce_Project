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
using System.Security.Claims;



namespace Ecommerce_Project.Controllers
{
    public class ProductCommentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public ProductCommentsController(
      ApplicationDbContext context,
      UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: ProductComments
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.productComments.Include(p => p.Product).Include(p => p.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ProductComments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productComment = await _context.productComments
                .Include(p => p.Product)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productComment == null)
            {
                return NotFound();
            }

            return View(productComment);
        }

        // GET: ProductComments/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: ProductComments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public async Task<IActionResult> Create(int productId, string comment)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            ProductComment newComment = new ProductComment
            {
                ProductId = productId,
                Comment = comment,
                UserId = user.Id,
                CreatedAt = DateTime.Now,
                IsApproved = true
            };

            _context.productComments.Add(newComment);

            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Products", new { id = productId });
        }

        // GET: ProductComments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productComment = await _context.productComments.FindAsync(id);
            if (productComment == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", productComment.ProductId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", productComment.UserId);
            return View(productComment);
        }

        // POST: ProductComments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Comment,IsApproved,CreatedAt,UserId,ProductId")] ProductComment productComment)
        {
            if (id != productComment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productComment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductCommentExists(productComment.Id))
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
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", productComment.ProductId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", productComment.UserId);
            return View(productComment);
        }

        // GET: ProductComments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productComment = await _context.productComments
                .Include(p => p.Product)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productComment == null)
            {
                return NotFound();
            }

            return View(productComment);
        }

        // POST: ProductComments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productComment = await _context.productComments.FindAsync(id);
            if (productComment != null)
            {
                _context.productComments.Remove(productComment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductCommentExists(int id)
        {
            return _context.productComments.Any(e => e.Id == id);
        }
    }
}

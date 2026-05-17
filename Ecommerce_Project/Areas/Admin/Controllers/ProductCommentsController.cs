using Ecommerce_Project.Data;
using Ecommerce_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce_Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
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

        // GET: Admin/ProductComments
        public async Task<IActionResult> Index()
        {
            var comments = await _context.productComments
                .Include(c => c.Product)
                .Include(c => c.User)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            var reviewQueue = comments.Select(c => new ReviewItemViewModel
            {
                CommentId = c.Id,
                CommentText = c.Comment,
                CreatedAt = c.CreatedAt,
                IsApproved = c.IsApproved,
                CustomerName = c.User?.FullName ?? "Unknown Client",
                CustomerEmail = c.User?.Email ?? "N/A",
                ProductName = c.Product?.Name ?? "Deleted Product"
            }).ToList();

            return View(reviewQueue);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleApproval(int id)
        {
            var comment = await _context.productComments.FindAsync(id);

            if (comment == null)
                return NotFound();

            comment.IsApproved = !comment.IsApproved;

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                approved = comment.IsApproved
            });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.productComments.FindAsync(id);

            if (comment == null)
            {
                return Json(new { success = false });
            }

            _context.productComments.Remove(comment);

            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ecommerce_Project.Data;
using Ecommerce_Project.Models;
using Microsoft.AspNetCore.Authorization;

namespace Ecommerce_Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class TestimonialsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TestimonialsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Testimonials
        public async Task<IActionResult> Index()
        {
            // Assuming your Testimonial model has an IsDeleted flag. 
            // If it doesn't yet, remove the Where condition until you add the migration.
            var testimonials = await _context.testimonials
                .Include(t => t.User)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return View(testimonials);
        }

        // POST: Admin/Testimonials/ToggleApproval
        [HttpPost]
        public async Task<IActionResult> ToggleApproval(int id)
        {
            var testimonial = await _context.testimonials.FindAsync(id);
            if (testimonial == null) return NotFound();

            // Seamlessly flip status between approved and pending
            testimonial.IsApproved = !testimonial.IsApproved;
            await _context.SaveChangesAsync();

            return Json(new { success = true, approved = testimonial.IsApproved });
        }

        // POST: Admin/Testimonials/SoftDelete
        [HttpPost]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var testimonial = await _context.testimonials.FindAsync(id);
            if (testimonial == null) return Json(new { success = false });

            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        // GET: Admin/Testimonials/Archive
        [HttpGet]
        public async Task<IActionResult> Archive()
        {
            // Isolate ONLY soft-deleted testimonials
            var archived = await _context.testimonials
                .Include(t => t.User)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return View(archived);
        }

        // POST: Admin/Testimonials/Restore
        [HttpPost]
        public async Task<IActionResult> Restore(int id)
        {
            var testimonial = await _context.testimonials.FindAsync(id);
            if (testimonial == null) return Json(new { success = false, message = "Testimonial not found." });

            // Flip the soft-delete tracking flag back to false
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
    }
}
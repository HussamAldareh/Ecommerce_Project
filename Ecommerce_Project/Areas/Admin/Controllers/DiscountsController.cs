using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ecommerce_Project.Data;
using Ecommerce_Project.Models;
using Microsoft.AspNetCore.Authorization;

namespace Ecommerce_Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DiscountsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DiscountsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Discounts
        public async Task<IActionResult> Index()
        {
            var discounts = await _context.Discount
                .Include(d => d.Product)
                .OrderByDescending(d => d.StartDate)
                .ToListAsync();
            return View(discounts);
        }

        // GET: Admin/Discounts/Create
        [Area("Admin")]
        public IActionResult Create()
        {
            // Pass the Name property as the Display Text instead of the Id property!
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name"); return View();
        }

        // POST: Admin/Discounts/ToggleActive
        [HttpPost]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var discount = await _context.Discount.FindAsync(id);
            if (discount == null) return NotFound();

            discount.IsActive = !discount.IsActive;
            await _context.SaveChangesAsync();

            return Json(new { success = true, isActive = discount.IsActive });
        }

        // POST: Admin/Discounts/DeleteConfirmed
        [HttpPost]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            var discount = await _context.Discount.FindAsync(id);
            if (discount == null) return Json(new { success = false });

            _context.Discount.Remove(discount);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ecommerce_Project.Data;
using Ecommerce_Project.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Ecommerce_Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Orders
        // GET: Orders
        public async Task<IActionResult> Index(string? userId)
        {
            // Start with all orders and include the User data for the names/emails
            var query = _context.orders
                .Include(o => o.User)
                .AsQueryable();

            // FEATURE: View orders by specific user
            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(o => o.UserId == userId);
                var targetUser = await _context.Users.FindAsync(userId);
                ViewData["FilterTitle"] = $"Orders for {targetUser?.UserName}";
            }
            else
            {
                ViewData["FilterTitle"] = "All Orders Entries";
            }

            return View(await query.OrderByDescending(o => o.OrderDate).ToListAsync());
        }

        // POST: Orders/UpdateStatus
        [HttpPost]

        public async Task<IActionResult> UpdateStatus([FromBody] OrderStatusUpdate data)
        {
            if (data == null) return BadRequest();

            var order = await _context.orders.FindAsync(data.Id);
            if (order == null) return NotFound();

            order.Status = data.Status;
            _context.Update(order);
            await _context.SaveChangesAsync();

            // Recalculate totals for the dashboard cards
            var allOrders = await _context.orders.ToListAsync();
            var stats = new
            {
                success = true,
                totalValue = allOrders.Sum(o => o.TotalAmount).ToString("N0"),
                completedCount = allOrders.Count(o => o.Status == "Completed"),
                processingCount = allOrders.Count(o => o.Status == "Processing"),
                cancelledCount = allOrders.Count(o => o.Status == "Cancelled")
            };

            return Ok(stats);
        }
        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                      .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }




        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,OrderDate,TotalAmount,Status,ShippingAddress,UserId")] Order order)
        //{
        //    if (id != order.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(order);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!OrderExists(order.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", order.UserId);
        //    return View(order);
        //}



        public async Task<IActionResult> OrderDetails(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = await _context.orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }






        public class OrderStatusUpdate
        {
            public int Id { get; set; }
            public string Status { get; set; }
        }
    }
}

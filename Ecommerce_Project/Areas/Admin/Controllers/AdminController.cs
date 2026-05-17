using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Ecommerce_Project.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Ecommerce_Project.Models;

namespace Ecommerce_Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> UserManager)
        {
            _context = context;
            _userManager = UserManager;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalUsers = await _context.Users.CountAsync();
            ViewBag.TotalProducts = await _context.Products.CountAsync();
            ViewBag.TotalOrders = await _context.orders.CountAsync();
            ViewBag.TotalCategories = await _context.Categories.CountAsync();

            var recentOrders = await _context.orders
                .Include(o => o.User)
                .OrderByDescending(o => o.OrderDate)
                .Take(5)
                .ToListAsync();
            var adminIds = (await _userManager.GetUsersInRoleAsync("Admin"))
                .Select(u => u.Id).ToList();

            var customerCount = await _context.Users
                .Where(u => !adminIds.Contains(u.Id))
                .CountAsync();

            // Get all orders for the current year
            var currentYear = DateTime.Now.Year;
            var orders = await _context.orders
                .Where(o => o.OrderDate.Year == currentYear && o.Status != "Cancelled")
                .ToListAsync();

            // Initialize an array of 12 decimals (one for each month)
            decimal[] monthlyRevenue = new decimal[12];

            for (int i = 0; i < 12; i++)
            {
                // Months are 1-indexed in C# (Jan = 1, Feb = 2...)
                monthlyRevenue[i] = orders
                    .Where(o => o.OrderDate.Month == i + 1)
                    .Sum(o => o.TotalAmount);
            }
            ViewBag.CustomerCount = customerCount;
            ViewBag.RevenueData = monthlyRevenue;

            return View(recentOrders);
        }
    }
}

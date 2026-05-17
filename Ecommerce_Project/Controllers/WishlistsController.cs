using Ecommerce_Project.Data;
using Ecommerce_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
namespace Ecommerce_Project.Controllers
{
    [Authorize]

    public class WishlistsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WishlistsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> MyWishlist()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var wishlist = await _context.Wishlist
                .Include(w => w.WishlistItems)
                .ThenInclude(wi => wi.Product)
                .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wishlist == null)
            {
                wishlist = new Wishlist
                {
                    WishlistItems = new List<WishlistItem>()
                };
            }

            return View(wishlist);
        }


        [HttpPost]
        public async Task<IActionResult> AddToWishlist(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
            {
                return NotFound();
            }

            var wishlist = await _context.Wishlist
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wishlist == null)
            {
                wishlist = new Wishlist
                {
                    UserId = userId
                };

                _context.Wishlist.Add(wishlist);
                await _context.SaveChangesAsync();
            }

            var existingItem = await _context.wishlistItems
                .FirstOrDefaultAsync(w =>
                    w.WishlistId == wishlist.Id &&
                    w.ProductId == productId);

            if (existingItem == null)
            {
                WishlistItem item = new WishlistItem
                {
                    WishlistId = wishlist.Id,
                    ProductId = productId
                };

                _context.wishlistItems.Add(item);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("MyWishlist");
        }




        public async Task<IActionResult> RemoveFromWishlist(int id)
        {
            var item = await _context.wishlistItems.FindAsync(id);


            if (item != null)
            {
                _context.wishlistItems.Remove(item);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("MyWishlist");
        }





        // GET: Wishlists
    }
}
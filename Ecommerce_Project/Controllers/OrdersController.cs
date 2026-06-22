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
using Stripe.Checkout;
namespace Ecommerce_Project.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.orders.Include(o => o.User);
            return View(await applicationDbContext.ToListAsync());
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
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OrderDate,TotalAmount,Status,ShippingAddress,UserId")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Name", order.UserId);
            return View(order);
        }
            
        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", order.UserId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrderDate,TotalAmount,Status,ShippingAddress,UserId")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", order.UserId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.orders.FindAsync(id);
            if (order != null)
            {
                _context.orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.orders.Any(e => e.Id == id);
        }



        public async Task<IActionResult> MyOrders() { 
        
        
        var userId=User.FindFirstValue(ClaimTypes.NameIdentifier);

            var orders = await _context.orders.Where(o => o.UserId == userId).ToListAsync();

            return View (orders);
        
        
        }


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





        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .ThenInclude(p => p.Discounts)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
            {
                return RedirectToAction("MyCart", "CartItems");
            }

            decimal total = 0;

            foreach (var item in cart.CartItems)
            {
                var activeDiscount = item.Product.Discounts?
                    .FirstOrDefault(d =>
                        d.IsActive &&
                        d.StartDate <= DateTime.Now &&
                        d.EndDate >= DateTime.Now);

                decimal finalPrice = item.Product.Price;

                if (activeDiscount != null)
                {
                    finalPrice = item.Product.Price -
                        (item.Product.Price * activeDiscount.Percentage / 100);
                }

                total += item.Quantity * finalPrice;
            }

            Order order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                Status = "Processing",
                ShippingAddress = "Amman",
                TotalAmount = total
            };

            _context.orders.Add(order);

            await _context.SaveChangesAsync();

            foreach (var item in cart.CartItems)
            {
                var activeDiscount = item.Product.Discounts?
                    .FirstOrDefault(d =>
                        d.IsActive &&
                        d.StartDate <= DateTime.Now &&
                        d.EndDate >= DateTime.Now);

                decimal finalPrice = item.Product.Price;

                if (activeDiscount != null)
                {
                    finalPrice = item.Product.Price -
                        (item.Product.Price * activeDiscount.Percentage / 100);
                }

                OrderItem orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = finalPrice
                };
                  
                _context.orderItems.Add(orderItem);
            }

            _context.CartItems.RemoveRange(cart.CartItems);

            await _context.SaveChangesAsync();

            return RedirectToAction("Payment", new { id = order.Id });
        }

       
        public async Task<IActionResult> Payment(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = await _context.orders
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null)
            {
                return NotFound();
            }

            var domain = "https://localhost:7061/";

            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + $"Orders/Success?orderId={order.Id}",
                CancelUrl = domain + $"Orders/Cancel?orderId={order.Id}",
                Mode = "payment",

                LineItems = new List<SessionLineItemOptions>
        {
            new SessionLineItemOptions
            {
                Quantity = 1,

                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "usd",

                    UnitAmount = (long)(order.TotalAmount * 100),

                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = $"Order #{order.Id}"
                    }
                }
            }
        }
            };

            var service = new SessionService();

            Session session = service.Create(options);

            return Redirect(session.Url);
        }


        public async Task<IActionResult> Success(int orderId)
        {
            var order = await _context.orders.FindAsync(orderId);

            if (order == null)
            {
                return NotFound();
            }

            Payment payment = new Payment
            {
                OrderId = order.Id,
                PaymentMethod = "Visa",
                PaymentStatus = "Paid",
                PaymentDate = DateTime.Now
            };

            _context.payments.Add(payment);

            order.Status = "Completed";

            await _context.SaveChangesAsync();

            return RedirectToAction("MyOrders");
        }
        public IActionResult Cancel(int orderId)
        {
            return Content("Payment Cancelled");
        }

    }
}

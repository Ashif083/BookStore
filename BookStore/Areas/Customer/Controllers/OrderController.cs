using BookStore.Data;
using BookStore.Models;
using BookStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;
using BookStore.Areas.Customer.Model;
using System.Diagnostics;

namespace BookStore.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        public OrderController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        //Get
        public async Task<IActionResult> Checkout(Order order)
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);

                if (user != null && user is ApplicationUsers appUser)
                {
                    order.Name = appUser.FirstName + " " + appUser.LastName;
                    order.Email = appUser.Email;
                    order.Phone = appUser.PhoneNumber;
                    order.Address = appUser.Address;
                }
            }
            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> CheckoutPost(Order order)
        {
            List<Books> book = HttpContext.Session.Get<List<Books>>("book");
            order.OrderDetails = new List<OrderDetails>();
            if (book != null)
            {
                foreach (var item in book)
                {
                    OrderDetails orderDetails = new OrderDetails();
                    orderDetails.BookId = item.Id;

                    order.OrderDetails.Add(orderDetails);

                    var bookToUpdate = await _db.Books.FirstOrDefaultAsync(b => b.Id == item.Id);
                    if (bookToUpdate != null)
                    {
                        bookToUpdate.AvailableQuantity -= 1;
                    }
                }
            }
            order.OrderNo = GetOrderNo();
            var userId = _userManager.GetUserId(User);
            order.CustomerId = userId;
            _db.Order.Add(order);
            await _db.SaveChangesAsync();

            var domain = "https://localhost:44355/";
            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + $"Customer/Order/OrderDetails/{order.Id}",
                CancelUrl = domain + $"Checkout/Login",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment"

            };
            foreach (var item in book)
            {
                var sessionListItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "bdt",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.BookName.ToString(),
                        },
                        UnitAmount = (long)(item.Price * 100)

                    },
                    Quantity = 1
                };
                options.LineItems.Add(sessionListItem);

            }
            var service = new SessionService();
            Session session = service.Create(options);

            Response.Headers.Add("Location", session.Url);
            HttpContext.Session.Set("book", new List<Books>());
            return new StatusCodeResult(303);

        }

        public string GetOrderNo()
        {
            int rowCount = _db.Order.ToList().Count() + 1;
            return rowCount.ToString("000");
        }
        public IActionResult OrderDetails(int id)
        {
            Order order = _db.Order.Include(o => o.OrderDetails).ThenInclude(od => od.Books).FirstOrDefault(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }



        public IActionResult OrderHistory()
        {
            var userId = _userManager.GetUserId(User); // Get the user's ID
            var orders = _db.Order
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Books)
                .Where(o => o.CustomerId == userId)
                .ToList();

            return View(orders);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AdminOrderHistory(string month)
        {
            var ordersQuery = _db.Order
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Books)
                .AsQueryable();

            if (!string.IsNullOrEmpty(month))
            {
                int selectedMonth = int.Parse(month);
                ordersQuery = ordersQuery.Where(o => o.OrderDate.Month == selectedMonth);
            }

            var orders = ordersQuery.ToList();

            return View(orders);
        }



    }

}
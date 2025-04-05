using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_SophieBakes.Data;
using Project_SophieBakes.Models;
using Project_SophieBakes.Extensions;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project_SophieBakes.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<CheckoutController> _logger;

        public CheckoutController(
            ApplicationDbContext context,
            UserManager<User> userManager,
            ILogger<CheckoutController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart");

                if (cart == null || !cart.Any())
                {
                    _logger.LogWarning("Attempt to access checkout with empty cart");
                    TempData["Error"] = "Your cart is empty!";
                    return RedirectToAction("Index", "Cart"); // Ensure this redirects to CartController
                }

                // Validate cart items against database
                var productIds = cart.Select(c => c.ProductId).ToList();
                var products = _context.Products.Where(p => productIds.Contains(p.ProductId)).ToList();

                if (products.Count != productIds.Count)
                {
                    _logger.LogWarning("Some products in cart no longer exist");
                    TempData["Error"] = "Some items in your cart are no longer available.";
                    return RedirectToAction("Index", "Cart"); // Ensure this redirects to CartController
                }

                return View(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Checkout Index");
                TempData["Error"] = "An error occurred. Please try again.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(List<CartItem> cartItems) //changed parameter name to cartItems
        {
            _logger.LogInformation("PlaceOrder method started.");

            try
            {
                //use cartItems parameter
                if (cartItems == null || !cartItems.Any())
                {
                    _logger.LogWarning("Attempt to place order with empty cart.");
                    return RedirectToAction("Index", "Cart");
                }

                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("User not authenticated during order placement.");
                    return Json(new { success = false, message = "Please log in to continue." });
                }

                await using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Validate product existence and prices
                    var productIds = cartItems.Select(c => c.ProductId).ToList();
                    var products = await _context.Products
                        .Where(p => productIds.Contains(p.ProductId))
                        .ToDictionaryAsync(p => p.ProductId, p => p);

                    foreach (var item in cartItems)
                    {
                        if (!products.ContainsKey(item.ProductId))
                        {
                            await transaction.RollbackAsync();
                            _logger.LogError("Product not found: ProductId = {ProductId}", item.ProductId);
                            return Json(new { success = false, message = "Some products are no longer available." });
                        }

                        if (products[item.ProductId].Price != item.Price)
                        {
                            await transaction.RollbackAsync();
                            _logger.LogError(
                                "Product price mismatch: ProductId = {ProductId}, Expected Price = {ExpectedPrice}, Actual Price = {ActualPrice}",
                                item.ProductId, item.Price, products[item.ProductId].Price);
                            return Json(new { success = false, message = "Product prices have changed. Please review your cart." });
                        }
                    }

                    // Create new order
                    var order = new Order
                    {
                        UserId = userId,
                        OrderDate = DateTime.UtcNow,
                        Status = "Pending",
                        TotalAmount = cartItems.Sum(i => i.Price * i.Quantity), // use cartItems
                        PaymentMethod = "Pending",
                        PaymentStatus = "Pending",
                        CustomerEmail = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty,
                        OrderItems = cartItems.Select(item => new OrderItem
                        {
                            ProductId = item.ProductId,
                            Quantity = item.Quantity,
                            Price = item.Price
                        }).ToList()
                    };

                    // Validate order model before saving
                    var orderValidationResults = new List<ValidationResult>();
                    var orderValidationContext = new ValidationContext(order);
                    if (!Validator.TryValidateObject(order, orderValidationContext, orderValidationResults, true))
                    {
                        await transaction.RollbackAsync();
                        foreach (var validationResult in orderValidationResults)
                        {
                            _logger.LogError("Order validation error: {ErrorMessage}", validationResult.ErrorMessage);
                        }

                        return Json(new { success = false, message = "Invalid order data." });
                    }

                    // Add Order to DB
                    _context.Orders.Add(order);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Order saved successfully. OrderId: {OrderId}", order.OrderId);

                    if (order.OrderId == 0)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError("Failed to save order to the database.");
                        return Json(new { success = false, message = "Failed to create order." });
                    }



                    await transaction.CommitAsync();

                    //clear cart
                    HttpContext.Session.Remove("Cart");
                    // Return success with the created orderId
                    return RedirectToAction("OrderConfirmation", new { orderId = order.OrderId });
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(dbEx, "Database error while placing order: {Message}, Inner Exception: {InnerException}",
                        dbEx.Message, dbEx.InnerException?.Message);

                    return Json(new { success = false, message = "A database error occurred while processing your order." });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Unexpected error while placing order: {Message}, Inner Exception: {InnerException}",
                        ex.Message, ex.InnerException?.Message);
                    return Json(new { success = false, message = "An unexpected error occurred while processing your order." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in PlaceOrder method.");
                return Json(new { success = false, message = "An unexpected error occurred." });
            }
        }

        public IActionResult OrderConfirmation(int orderId)
        {
            var order = _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product) // Ensures Product data is loaded
                .FirstOrDefault(o => o.OrderId == orderId);

            if (order == null)
            {
                _logger.LogWarning("OrderConfirmation: Order not found for OrderId {OrderId}", orderId);
                return NotFound("Order not found.");
            }

            return View(order);
        }

        [Authorize] // Ensure only logged-in users can access their orders
        public async Task<IActionResult> MyOrders()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }



    }
}


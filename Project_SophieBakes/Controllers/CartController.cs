using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Project_SophieBakes.Data;
using Project_SophieBakes.Models;
using Project_SophieBakes.Services;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Project_SophieBakes.Extensions;

namespace Project_SophieBakes.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ICompositeViewEngine _viewEngine;
        private readonly ILogger<CartController> _logger;

        public CartController(ApplicationDbContext context, UserManager<User> userManager,
            ICompositeViewEngine viewEngine, ILogger<CartController> logger)
        {
            _context = context;
            _userManager = userManager;
            _viewEngine = viewEngine;
            _logger = logger;
        }

        // ✅ Renders a Partial View as a string
        private string RenderPartialViewToString(string viewName, object model)
        {
            ViewData.Model = model;

            using (var writer = new StringWriter())
            {
                var viewResult = _viewEngine.FindView(ControllerContext, viewName, false);

                if (!viewResult.Success)
                {
                    throw new FileNotFoundException($"View '{viewName}' not found.");
                }

                var viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    ViewData,
                    TempData,
                    writer,
                    new HtmlHelperOptions()
                );

                viewResult.View.RenderAsync(viewContext).Wait();
                return writer.GetStringBuilder().ToString();
            }
        }

        // ✅ Get Cart Count (for updating UI)
        [HttpGet]
        public IActionResult GetCartCount()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart");
            int count = cart?.Sum(c => c.Quantity) ?? 0;
            return Json(count);
        }

        // ✅ View Cart Page
        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            if (cart.Any())
            {
                // Fetch product details from the database
                var productIds = cart.Select(c => c.ProductId).ToList();
                var products = _context.Products.Where(p => productIds.Contains(p.ProductId)).ToList();

                // Map product details to cart items
                foreach (var item in cart)
                {
                    item.Product = products.FirstOrDefault(p => p.ProductId == item.ProductId);
                }
            }

            return View(cart);
        }

        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public IActionResult DebugSession()
        {
            try
            {
                Console.WriteLine("🔹 DebugSession() method called"); // Step 1: Verify method is hit

                HttpContext.Session.SetString("TestKey", "Hello, session!");
                var sessionValue = HttpContext.Session.GetString("TestKey");

                Console.WriteLine($"🔍 Session Test: {sessionValue}"); // Step 2: Check session value

                return Content($"Session Value: {sessionValue}", "text/plain"); // Step 3: Ensure response
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error in DebugSession(): {ex.Message}");
                return Content($"Error: {ex.Message}", "text/plain");
            }
        }



        // ✅ Add Item to Cart
        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity = 1)
        {
            try
            {
                var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart");

                // If cart is null, create a new cart
                if (cart == null)
                {
                    cart = new List<CartItem>();
                    HttpContext.Session.SetObjectAsJson("Cart", cart);
                }

                // Debugging: Print cart contents before adding item
                Console.WriteLine("🛒 Cart Contents:");
                foreach (var item in cart)
                {
                    Console.WriteLine($" - Product ID: {item.ProductId}, Quantity: {item.Quantity}");
                }

                // Find the product in the database
                var product = _context.Products.FirstOrDefault(p => p.ProductId == productId);
                if (product == null)
                {
                    return Json(new { success = false, message = "Product not found" });
                }

                // Check if product already exists in the cart, if so, update the quantity
                var existingItem = cart.FirstOrDefault(c => c.ProductId == productId);
                if (existingItem != null)
                {
                    existingItem.Quantity += quantity; // Update quantity if the item already exists
                }
                else
                {
                    // Add new item to the cart
                    cart.Add(new CartItem
                    {
                        ProductId = product.ProductId,
                        ProductName = product.ProductName, // Ensure the name is correctly assigned
                        Price = product.Price, // Ensure price is correctly assigned
                        ImageUrl = product.ImageUrl ?? "/images/default.jpg", // Ensure image is assigned
                        Quantity = quantity
                    });
                }

                // Save the updated cart to session
                HttpContext.Session.SetObjectAsJson("Cart", cart);

                // Debugging: Print cart contents after adding item
                Console.WriteLine("🛒 Cart Updated:");
                foreach (var item in cart)
                {
                    Console.WriteLine(
                        $" - ProductId: {item.ProductId}, Name: {item.ProductName}, Price: {item.Price}, Quantity: {item.Quantity}, Image: {item.ImageUrl}");
                }

                return Json(new { success = true, message = "Item added to cart" });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error adding to cart: " + ex.Message);
                return Json(new { success = false, message = "An error occurred" });
            }
        }




        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            Console.WriteLine($"🔍 Searching for Product ID: {productId} in cart...");

            // Check if session has a cart
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart");

            if (cart == null || !cart.Any())
            {
                Console.WriteLine("❌ Cart is empty or does not exist in session.");
                return Json(new { success = false, message = "Cart is empty or session expired." });
            }

            // Print all products in the cart to debug
            Console.WriteLine("🛒 Current Cart Contents:");
            foreach (var item in cart)
            {
                Console.WriteLine($"🔹 Product ID: {item.ProductId}, Quantity: {item.Quantity}");
            }

            var cartItem = cart.FirstOrDefault(c => c.ProductId == productId);
            if (cartItem == null)
            {
                Console.WriteLine($"❌ Product with ID {productId} NOT found in cart.");
                return Json(new { success = false, message = "Product not found in cart." });
            }

            // Update quantity
            cartItem.Quantity = quantity;
            HttpContext.Session.SetObjectAsJson("Cart", cart);
            HttpContext.Session.SetInt32("CartCount", cart.Sum(i => i.Quantity));

            Console.WriteLine($"✅ Updated product {productId} to quantity {quantity}.");

            var cartHtml = RenderPartialViewToString("_CartPartial", cart);
            return Json(new { success = true, cartHtml = cartHtml });
        }


        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var userId = int.Parse(user.Id);
                var cartItems = await _context.CartItems.Where(c => c.Id == userId).ToListAsync();
                _context.CartItems.RemoveRange(cartItems);
                await _context.SaveChangesAsync();
            }

            return Json(new { success = true });
        }



        // ✅ Remove Item from Cart
        [HttpPost]
        public IActionResult RemoveItem([FromBody] CartItemDto itemDto)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            var itemToRemove = cart.FirstOrDefault(i => i.ProductId == itemDto.ProductId);
            if (itemToRemove != null)
            {
                cart.Remove(itemToRemove);
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }

            return Json(new
            {
                success = true,
                cartHtml = RenderPartialViewToString("_CartPartial", cart), // ✅ Use the existing method
                total = cart.Sum(i => i.Quantity * (i.Product?.Price ?? 0)),
                cartCount = cart.Count
            });



        }

        [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> PlaceOrder()
{
    _logger.LogInformation("PlaceOrder method started.");
    try
    {
        var cartItems = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart");

        if (cartItems == null || !cartItems.Any())
        {
            _logger.LogWarning("Attempt to place order with empty cart");
            return Json(new { success = false, message = "Your cart is empty!" });
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            _logger.LogWarning("User not authenticated during order placement");
            return Json(new { success = false, message = "Please log in to continue." });
        }
        var userId = user.Id;

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Validate products and prices
            var productIds = cartItems.Select(c => c.ProductId).ToList();
            var products = await _context.Products
                .Where(p => productIds.Contains(p.ProductId))
                .ToDictionaryAsync(p => p.ProductId, p => p);

            foreach (var item in cartItems)
            {
                if (!products.ContainsKey(item.ProductId))
                {
                    await transaction.RollbackAsync();
                    _logger.LogError($"Product not found: ProductId = {item.ProductId}");
                    return Json(new { success = false, message = "Some products are no longer available." });
                }

                if (products[item.ProductId].Price != item.Price)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError($"Product price mismatch: ProductId = {item.ProductId}, Expected Price = {item.Price}, Actual Price = {products[item.ProductId].Price}");
                    return Json(new { success = false, message = "Product prices have changed. Please review your cart." });
                }
            }

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                Status = "Pending",
                TotalAmount = cartItems.Sum(i => i.Price * i.Quantity),
                PaymentMethod = "Pending",  //  Set appropriate values
                PaymentStatus = "Pending", //   Set appropriate values
                CustomerEmail = user.Email // Set the customer email
            };

            // Validate the order model
            var orderValidationContext = new ValidationContext(order);
            var orderValidationResults = new List<ValidationResult>();

            if (!Validator.TryValidateObject(order, orderValidationContext, orderValidationResults, true))
            {
                await transaction.RollbackAsync();
                foreach (var validationResult in orderValidationResults)
                {
                    _logger.LogError($"Order validation error: {validationResult.ErrorMessage}");
                }
                _logger.LogError("Order object before validation: {@Order}", order);
                return Json(new { success = false, message = "Invalid order data." });
            }

            _logger.LogInformation("Order object before saving: {@Order}", order); // Add this
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Order saved with OrderId: {OrderId}", order.OrderId); // Add this

            if (order.OrderId == 0)
            {
                await transaction.RollbackAsync();
                _logger.LogError("Failed to save order to the database. Order object: {@Order}", order);
                return Json(new { success = false, message = "Failed to create order." });
            }

            foreach (var item in cartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.OrderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price,
                };
                 var orderItemValidationContext = new ValidationContext(orderItem);
                        var orderItemValidationResults = new List<ValidationResult>();
                        if (!Validator.TryValidateObject(orderItem, orderItemValidationContext,
                                orderItemValidationResults, true))
                        {
                            await transaction.RollbackAsync();
                             foreach (var validationResult in orderItemValidationResults)
                            {
                                 _logger.LogError($"OrderItem validation error: {validationResult.ErrorMessage}");
                            }
                            _logger.LogError("OrderItem object before validation: {@OrderItem}", orderItem);
                            return Json(new { success = false, message = "Invalid order item data." });
                        }

                _context.OrderItems.Add(orderItem);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
             _logger.LogInformation("Transaction committed successfully. OrderId: {OrderId}", order.OrderId);
            return Json(new { success = true, orderId = order.OrderId });
        }
        catch (DbUpdateException dbEx)
        {
            await transaction.RollbackAsync();
            _logger.LogError(dbEx, "Database error occurred while placing order. Inner Exception: {InnerException}, StackTrace: {StackTrace}, Message: {Message}", dbEx.InnerException?.Message, dbEx.StackTrace, dbEx.Message);

            // Log the entries that caused the error
            foreach (var entry in dbEx.Entries)
            {
                _logger.LogError($"Entity of type {entry.Entity.GetType().Name} caused an error. State: {entry.State}");
                 foreach (var property in entry.Properties)
                {
                    _logger.LogError($"Property: {property.Metadata.Name}, Value: {property.CurrentValue}, OriginalValue: {property.OriginalValue},IsModified: {property.IsModified}");
                }
            }
            return Json(new { success = false, message = "A database error occurred while processing your order." });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error occurred while placing order. Exception: {ExceptionMessage}, InnerException: {InnerException}, StackTrace: {StackTrace}, Message: {Message}", ex.Message, ex.InnerException?.Message, ex.StackTrace, ex.Message);
            return Json(new { success = false, message = "An error occurred while processing your order." });
        }
    }
    catch (Exception ex) // Added this catch block to handle the outer try-catch issue
    {
         _logger.LogError(ex, "Unexpected error in PlaceOrder method.  Message: {Message}", ex.Message);
        return Json(new { success = false, message = "An unexpected error occurred." });
    }
}

    } 
    }


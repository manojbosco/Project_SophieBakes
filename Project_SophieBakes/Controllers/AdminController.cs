using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_SophieBakes.Models;
using Project_SophieBakes.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Project_SophieBakes.ViewModels;

namespace Project_SophieBakes.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminController(ILogger<AdminController> logger, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> CustomerOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.User) // Include user details if needed
                .Include(o => o.OrderItems) // Include ordered items
                .ThenInclude(oi => oi.Product) // Include product details
                .OrderByDescending(o => o.OrderDate) // Most recent orders first
                .ToListAsync();

            return View(orders);
        }
        
        public async Task<IActionResult> OrderDetails(int id)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

       

        public async Task<IActionResult> Feedback()
        {
            var feedbacks = await _context.Feedbacks
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            return View(feedbacks);
        }

        
        // GET: Admin/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Admin/Login
        [HttpPost]
        public IActionResult Login(AdminLoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Username == "admin" && model.Password == "admin123")
                {
                    HttpContext.Session.SetString("IsAdmin", "true");
                    return RedirectToAction("ManageProducts");
                }
                ModelState.AddModelError("", "Invalid login details");
            }
            return View(model);
        }

        public IActionResult AdminDashboard()
        {
            var products = _context.Products.ToList();
            return View(products);
        }

        // GET: Admin/ManageProducts
        public IActionResult ManageProducts()
        {
            if (!IsAdminAuthenticated()) return RedirectToAction("Login");
            return View(_context.Products.ToList());
        }

        // GET: Admin/AddProduct
        [HttpGet]
        public IActionResult AddProduct()
        {
            ViewBag.Categories = GetCategorySelectList();
            return View();
        }

        // POST: Admin/AddProduct
        [HttpPost]
        public async Task<IActionResult> AddProduct(Product product, IFormFile imageFile)
        {
            if (!IsAdminAuthenticated()) return RedirectToAction("AdminDashboard");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                _logger.LogError("Model validation failed: {Errors}", string.Join(", ", errors));
                ViewBag.Categories = GetCategorySelectList();
                return View(product);
            }


            try
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadFolder)) Directory.CreateDirectory(uploadFolder);
                    
                    string uniqueFileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
                    string imagePath = Path.Combine(uploadFolder, uniqueFileName);
                    
                    using (var fileStream = new FileStream(imagePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }
                    
                    product.ImageUrl = "/uploads/" + uniqueFileName;
                }
                else
                {
                    ModelState.AddModelError("ImageFile", "Please select an image.");
                    return View(product);
                }
                
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction("ManageProducts");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding product");
                ModelState.AddModelError("", "Failed to add product. Please try again.");
                return View(product);
            }
        }

        // POST: Admin/DeleteProduct/{id}
        [HttpPost]
        public IActionResult DeleteProduct(int id)
        {
            if (!IsAdminAuthenticated()) return RedirectToAction("Login");

            var product = _context.Products.Find(id);
            if (product == null) return NotFound();

            _context.Products.Remove(product);
            _context.SaveChanges();
            return RedirectToAction("ManageProducts");
        }

        public async Task<IActionResult> SalesReport(string userId)
        {
            var totalSales = _context.Orders.Sum(o => (decimal?)o.TotalAmount) ?? 0;
            decimal userSales = 0;
            if (!string.IsNullOrEmpty(userId))
            {
                userSales = _context.Orders.Where(o => o.UserId == userId).Sum(o => (decimal?)o.TotalAmount) ?? 0;
            }

            var report = new SalesReportViewModel
            {
                TotalSales = totalSales,
                UserSales = userSales,
                SelectedUserId = userId
            };

            return View(report);
        }

        private bool IsAdminAuthenticated()
        {
            return HttpContext.Session.GetString("IsAdmin") == "true";
        }

        private SelectList GetCategorySelectList()
        {
            return new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Cakes" },
                new SelectListItem { Value = "2", Text = "Cupcakes" },
                new SelectListItem { Value = "3", Text = "Cookies" },
                new SelectListItem { Value = "4", Text = "Others" }
            }, "Value", "Text");
        }

    }
}

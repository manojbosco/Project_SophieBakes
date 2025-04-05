using Microsoft.AspNetCore.Mvc;
using Project_SophieBakes.Models; // Make sure this is the correct namespace for your models
using System.Linq;
using Project_SophieBakes.Data;

namespace Project_SophieBakes.Controllers
{
    public class OrderConfirmationController : Controller
    {
        private readonly ApplicationDbContext _context;  //  Use your actual DbContext

        public OrderConfirmationController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int orderId)
        {
            // Retrieve the order details from the database
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == orderId);

            if (order == null)
            {
                return View("OrderNotFound");  // Or redirect to an error page
            }

            // Pass the order details to the view
            return View(order);
        }
    }
}
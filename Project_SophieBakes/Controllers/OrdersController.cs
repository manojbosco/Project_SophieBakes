using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_SophieBakes.Data;
using Project_SophieBakes.Models;
using Project_SophieBakes.ViewModel;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Linq;

[Authorize]
public class OrderController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;

    public OrderController(ApplicationDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("Order/ConfirmPayment")]
    public IActionResult ConfirmPayment(int orderId)
    {
        Console.WriteLine($"Received Order ID: {orderId}");

        var order = _context.Orders
            .FirstOrDefault(o => o.OrderId == orderId);

        if (order == null)
        {
            Console.WriteLine($"Order with ID {orderId} not found in database.");
            return NotFound(new { success = false, message = "Order not found." });
        }

        order.PaymentStatus = "Confirmed";
        _context.SaveChanges();

        return Json(new { success = true, redirectUrl = "/Order/Success" });
    }
    
}

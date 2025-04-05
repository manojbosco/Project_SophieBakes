using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_SophieBakes.Data;
using Project_SophieBakes.Models;
using Project_SophieBakes.ViewModels;
using System.Linq;
using System.Threading.Tasks;

public class DeliveryBoyController : Controller
{
    private readonly ApplicationDbContext _context;

    public DeliveryBoyController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // Include the related User data when querying the Orders
        var orders = await _context.Orders
            .Include(o => o.User)  // Ensure we are including related User details
            .Where(o => o.Status == "Pending")  // Only retrieve pending orders
            .Select(o => new DeliveryOrders
            {
                Id = o.OrderId, // Correctly referencing OrderId
                CustomerName = o.User.Name,  // Fetching the full name of the customer
                Address = o.User.Address, // Fetching the address of the user
                PhoneNumber = o.User.PhoneNumber, // Fetching the phone number
                Status = o.Status,
                UserId = o.UserId // Including the UserId in the result
            })
            .ToListAsync();

        return View(orders);  // Passing the orders to the view
    }

    public async Task<IActionResult> MarkAsDelivered(int id)
    {
        var order = await _context.Orders.FindAsync(id);  // Find order by OrderId
        if (order != null)
        {
            order.Status = "Delivered";  // Update the status to Delivered
            await _context.SaveChangesAsync();  // Save changes to the database
        }

        return RedirectToAction("Index");  // Redirect to the index page after updating
    }
}
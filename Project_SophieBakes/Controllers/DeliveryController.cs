using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "DeliveryBoy")]
public class DeliveryController : Controller
{
    public IActionResult Dashboard()
    {
        return View();
    }
}
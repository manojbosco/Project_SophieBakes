using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Linq;
using Project_SophieBakes.Data;
using Project_SophieBakes.Models;

public class FeedbackController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager; // ✅ Changed IdentityUser to User

    public FeedbackController(ApplicationDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var feedbacks = await _context.Feedbacks
            .OrderByDescending(f => f.CreatedAt) // Ensure CreatedAt exists in Feedback model
            .ToListAsync();

        return View(feedbacks);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> SubmitFeedback(string comment, int rating)
    {
        if (rating < 1 || rating > 5 || string.IsNullOrWhiteSpace(comment))
        {
            TempData["Error"] = "Invalid feedback!";
            return RedirectToAction("Index");
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            TempData["Error"] = "User not found!";
            return RedirectToAction("Index");
        }

        var feedback = new Feedback
        {
            UserId = user.Id,
            UserName = user.UserName,
            Comment = comment,
            Rating = rating,
            CreatedAt = DateTime.UtcNow // ✅ Ensure timestamp is set
        };

        _context.Feedbacks.Add(feedback);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Thank you for your feedback!";
        return RedirectToAction("Index");
    }
}
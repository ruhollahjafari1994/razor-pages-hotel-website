using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RoohollahJafari.Web.Data;
using RoohollahJafari.Web.Models;

namespace RoohollahJafari.Web.Pages;

public class AboutModel : PageModel
{
    private readonly AppDbContext _db;
    public AboutModel(AppDbContext db) => _db = db;

    [BindProperty] public string Name { get; set; } = string.Empty;
    [BindProperty] public string Email { get; set; } = string.Empty;
    [BindProperty] public DateTime? CheckIn { get; set; }
    [BindProperty] public DateTime? CheckOut { get; set; }
    [BindProperty] public string? Message { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Email) || !Email.Contains('@'))
        {
            TempData["Error"] = "Please fill in your name and a valid email address.";
            return RedirectToPage();
        }

        _db.BookingRequests.Add(new BookingRequest
        {
            Name = Name.Trim(),
            Email = Email.Trim(),
            CheckIn = CheckIn,
            CheckOut = CheckOut,
            Details = Message?.Trim()
        });
        await _db.SaveChangesAsync();

        TempData["Success"] = "Your booking request has been received! We'll confirm availability by email.";
        return RedirectToPage();
    }
}

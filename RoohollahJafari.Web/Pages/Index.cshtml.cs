using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RoohollahJafari.Web.Data;
using RoohollahJafari.Web.Models;

namespace RoohollahJafari.Web.Pages;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    public IndexModel(AppDbContext db) => _db = db;

    // hero search form
    [BindProperty] public DateTime? CheckIn { get; set; }
    [BindProperty] public DateTime? CheckOut { get; set; }
    [BindProperty] public string? Rooms { get; set; }
    [BindProperty] public string? Guests { get; set; }

    // booking form
    [BindProperty] public string Name { get; set; } = string.Empty;
    [BindProperty] public string Email { get; set; } = string.Empty;
    [BindProperty] public string? Message { get; set; }

    public IActionResult OnPostSearch()
    {
        var url = "/rooms.html";
        if (CheckIn.HasValue || CheckOut.HasValue)
        {
            var query = new List<string>();
            if (CheckIn.HasValue) query.Add("checkin=" + CheckIn.Value.ToString("yyyy-MM-dd"));
            if (CheckOut.HasValue) query.Add("checkout=" + CheckOut.Value.ToString("yyyy-MM-dd"));
            if (!string.IsNullOrWhiteSpace(Rooms)) query.Add("rooms=" + Uri.EscapeDataString(Rooms));
            if (!string.IsNullOrWhiteSpace(Guests)) query.Add("guests=" + Uri.EscapeDataString(Guests));
            url += "?" + string.Join("&", query);
        }
        return Redirect(url);
    }

    public async Task<IActionResult> OnPostBooking()
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

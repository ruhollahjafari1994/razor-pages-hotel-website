using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RoohollahJafari.Web.Data;
using RoohollahJafari.Web.Models;

namespace RoohollahJafari.Web.Pages;

public class RoomDetailsModel : PageModel
{
    private readonly AppDbContext _db;
    public RoomDetailsModel(AppDbContext db) => _db = db;

    [BindProperty] public DateTime? CheckIn { get; set; }
    [BindProperty] public DateTime? CheckOut { get; set; }
    [BindProperty] public string? RoomType { get; set; }
    [BindProperty] public string? Guests { get; set; }
    [BindProperty] public string? Children { get; set; }
    [BindProperty] public string[]? Services { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        var details = new List<string>();
        if (!string.IsNullOrWhiteSpace(RoomType)) details.Add("Rooms: " + RoomType);
        if (!string.IsNullOrWhiteSpace(Guests)) details.Add("Adults: " + Guests);
        if (!string.IsNullOrWhiteSpace(Children)) details.Add("Children: " + Children);
        if (Services is { Length: > 0 }) details.Add("Extra services: " + string.Join(", ", Services));

        _db.BookingRequests.Add(new BookingRequest
        {
            Name = "Room booking",
            Email = "booking@roohollah-jafari.ir",
            CheckIn = CheckIn,
            CheckOut = CheckOut,
            Details = details.Count > 0 ? string.Join(" | ", details) : "Submitted from room details page"
        });
        await _db.SaveChangesAsync();

        TempData["Success"] = "Your booking request has been received! We'll confirm availability by email.";
        return RedirectToPage();
    }
}

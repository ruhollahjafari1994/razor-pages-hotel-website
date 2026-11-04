using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RoohollahJafari.Web.Data;
using RoohollahJafari.Web.Models;

namespace RoohollahJafari.Web.Pages;

public class Index3Model : PageModel
{
    private readonly AppDbContext _db;
    public Index3Model(AppDbContext db) => _db = db;

    [BindProperty] public DateTime? CheckIn { get; set; }
    [BindProperty] public DateTime? CheckOut { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        _db.BookingRequests.Add(new BookingRequest
        {
            Name = "Website booking form",
            Email = "booking@roohollah-jafari.ir",
            CheckIn = CheckIn,
            CheckOut = CheckOut,
            Details = "Submitted from Home Three booking form"
        });
        await _db.SaveChangesAsync();

        TempData["Success"] = "Your booking request has been received! We'll confirm availability by email.";
        return RedirectToPage();
    }
}

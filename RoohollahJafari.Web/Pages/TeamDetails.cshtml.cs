using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RoohollahJafari.Web.Data;
using RoohollahJafari.Web.Models;

namespace RoohollahJafari.Web.Pages;

public class TeamDetailsModel : PageModel
{
    private readonly AppDbContext _db;
    public TeamDetailsModel(AppDbContext db) => _db = db;

    [BindProperty] public string Name { get; set; } = string.Empty;
    [BindProperty] public string Email { get; set; } = string.Empty;
    [BindProperty] public string? Subject { get; set; }
    [BindProperty] public string? Phone { get; set; }
    [BindProperty] public string? Message { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Email) || !Email.Contains('@'))
        {
            TempData["Error"] = "Please fill in your name and a valid email address.";
            return RedirectToPage();
        }

        _db.ContactMessages.Add(new ContactMessage
        {
            Name = Name.Trim(),
            Email = Email.Trim(),
            Subject = Subject?.Trim(),
            Phone = Phone?.Trim(),
            Message = Message?.Trim()
        });
        await _db.SaveChangesAsync();

        TempData["Success"] = "Thanks! Your message has been sent — we'll get back to you soon.";
        return RedirectToPage();
    }
}

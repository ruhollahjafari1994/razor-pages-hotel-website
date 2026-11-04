using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RoohollahJafari.Web.Data;
using RoohollahJafari.Web.Models;

namespace RoohollahJafari.Web.Pages;

public class BlogDetailsModel : PageModel
{
    private readonly AppDbContext _db;
    public BlogDetailsModel(AppDbContext db) => _db = db;

    [BindProperty] public string Name { get; set; } = string.Empty;
    [BindProperty] public string Email { get; set; } = string.Empty;
    [BindProperty] public string? Message { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Email) || !Email.Contains('@'))
        {
            TempData["Error"] = "Please fill in your name and a valid email address.";
            return RedirectToPage();
        }

        _db.BlogComments.Add(new BlogComment
        {
            Name = Name.Trim(),
            Email = Email.Trim(),
            Message = Message?.Trim()
        });
        await _db.SaveChangesAsync();

        TempData["Success"] = "Your comment has been posted, thank you!";
        return RedirectToPage();
    }
}

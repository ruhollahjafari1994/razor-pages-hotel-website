using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RoohollahJafari.Web.Pages;

public class BlogGridModel : PageModel
{
    /// <summary>Search box on the blog pages posts here as a GET (?search=...).</summary>
    public void OnGet(string? search)
    {
        if (!string.IsNullOrWhiteSpace(search))
        {
            ViewData["Search"] = search;
        }
    }
}

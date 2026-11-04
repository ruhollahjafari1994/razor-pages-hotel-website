using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RoohollahJafari.Web.Pages;

public class RoomsModel : PageModel
{
    /// <summary>The hero search form on the home page redirects here with dates.</summary>
    public void OnGet(string? checkin, string? checkout, string? rooms, string? guests)
    {
        if (checkin is null && checkout is null && rooms is null && guests is null) return;

        var parts = new List<string>();
        if (DateTime.TryParse(checkin, out var ci)) parts.Add(ci.ToString("MMM d, yyyy"));
        if (DateTime.TryParse(checkout, out var co)) parts.Add(co.ToString("MMM d, yyyy"));
        var range = parts.Count > 0 ? string.Join(" → ", parts) : "any dates";

        var extra = new List<string>();
        if (!string.IsNullOrWhiteSpace(rooms)) extra.Add(rooms.Replace("0", "") + " room(s)");
        if (!string.IsNullOrWhiteSpace(guests)) extra.Add(guests);

        var suffix = extra.Count > 0 ? " · " + string.Join(", ", extra) : "";
        TempData["SearchInfo"] = $"Search results for stays {range}{suffix}.";
    }
}

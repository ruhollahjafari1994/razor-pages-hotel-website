using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RoohollahJafari.Web.Data;
using RoohollahJafari.Web.Models;

namespace RoohollahJafari.Web.Pages.Admin;

public class MessagesModel : PageModel
{
    private readonly AppDbContext _db;
    public MessagesModel(AppDbContext db) => _db = db;

    public List<ContactMessage> Contacts { get; private set; } = [];
    public List<BookingRequest> Bookings { get; private set; } = [];
    public List<BlogComment> Comments { get; private set; } = [];
    public List<Subscriber> Subscribers { get; private set; } = [];

    public async Task OnGetAsync()
    {
        Contacts = await _db.ContactMessages.OrderByDescending(m => m.CreatedAt).Take(25).ToListAsync();
        Bookings = await _db.BookingRequests.OrderByDescending(b => b.CreatedAt).Take(25).ToListAsync();
        Comments = await _db.BlogComments.OrderByDescending(c => c.CreatedAt).Take(25).ToListAsync();
        Subscribers = await _db.Subscribers.OrderByDescending(s => s.CreatedAt).Take(25).ToListAsync();
    }
}

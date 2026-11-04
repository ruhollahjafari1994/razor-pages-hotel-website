using Microsoft.EntityFrameworkCore;
using RoohollahJafari.Web.Models;

namespace RoohollahJafari.Web.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();
    public DbSet<BookingRequest> BookingRequests => Set<BookingRequest>();
    public DbSet<BlogComment> BlogComments => Set<BlogComment>();
    public DbSet<Subscriber> Subscribers => Set<Subscriber>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ContactMessage>().HasIndex(m => m.CreatedAt);
        modelBuilder.Entity<BookingRequest>().HasIndex(b => b.CreatedAt);
        // Check-in / check-out are calendar dates, not timestamps — the `date`
        // type also avoids PostgreSQL's "UTC only" rule for timestamptz.
        modelBuilder.Entity<BookingRequest>().Property(b => b.CheckIn).HasColumnType("date");
        modelBuilder.Entity<BookingRequest>().Property(b => b.CheckOut).HasColumnType("date");
        modelBuilder.Entity<BlogComment>().HasIndex(c => c.CreatedAt);
        modelBuilder.Entity<Subscriber>().HasIndex(s => s.Email).IsUnique();
    }

    /// <summary>Creates the schema and adds demo rows when the database is empty.</summary>
    public async Task EnsureSeededAsync()
    {
        // Docker startup race: postgres can report healthy while still refusing
        // connections for a moment, so retry until the schema can be created.
        for (var attempt = 0; ; attempt++)
        {
            try
            {
                await Database.EnsureCreatedAsync();
                break;
            }
            catch (Exception) when (attempt < 30)
            {
                await Task.Delay(1000);
            }
        }

        if (!await ContactMessages.AnyAsync())
        {
            ContactMessages.AddRange(
                new ContactMessage { Name = "Sara Ahmadi", Email = "sara@example.com", Phone = "+98 912 000 1122", Subject = "Reservation", Message = "Do you have a sea-view suite available next week?", CreatedAt = DateTime.UtcNow.AddDays(-3) },
                new ContactMessage { Name = "John Miller", Email = "john@example.com", Phone = "+1 555 010 2233", Subject = "Conference", Message = "We need a hall for a 40-person conference.", CreatedAt = DateTime.UtcNow.AddDays(-1) });
            await SaveChangesAsync();
        }

        if (!await BookingRequests.AnyAsync())
        {
            BookingRequests.Add(new BookingRequest
            {
                Name = "Sara Ahmadi",
                Email = "sara@example.com",
                CheckIn = DateTime.UtcNow.AddDays(7),
                CheckOut = DateTime.UtcNow.AddDays(11),
                Details = "Deluxe room, 2 adults",
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            });
            await SaveChangesAsync();
        }

        if (!await BlogComments.AnyAsync())
        {
            BlogComments.Add(new BlogComment { Name = "Sara Ahmadi", Email = "sara@example.com", Message = "Beautiful views! We booked after reading this.", CreatedAt = DateTime.UtcNow.AddDays(-1) });
            await SaveChangesAsync();
        }

        if (!await Subscribers.AnyAsync())
        {
            Subscribers.Add(new Subscriber { Email = "newsletter@example.com" });
            await SaveChangesAsync();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using RoohollahJafari.Web.Data;
using RoohollahJafari.Web.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

// Database provider is chosen by configuration so the same image runs
// against SQL Server (default) or PostgreSQL (docker-compose).
var provider = builder.Configuration["Database:Provider"] ?? "SqlServer";
var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Connection string 'Default' is missing.");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (string.Equals(provider, "Postgres", StringComparison.OrdinalIgnoreCase))
    {
        options.UseNpgsql(connectionString);
    }
    else
    {
        options.UseSqlServer(connectionString);
    }
});

var app = builder.Build();

// ---- create schema + seed demo data on startup ----
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.EnsureSeededAsync();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/404.html");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Keep the original template URLs working: /index.html -> /
app.MapGet("/index.html", () => Results.Redirect("/", permanent: true));

// ------------------------------------------------------------------
// REST API
// ------------------------------------------------------------------
app.MapGet("/api/health", async (AppDbContext db) =>
{
    var dbUp = await db.Database.CanConnectAsync();
    return Results.Ok(new
    {
        status = dbUp ? "healthy" : "degraded",
        database = dbUp ? "up" : "down",
        provider,
        time = DateTime.UtcNow
    });
});

app.MapGet("/api/contact", async (AppDbContext db) =>
    Results.Ok(await db.ContactMessages.OrderByDescending(m => m.CreatedAt).Take(50).ToListAsync()));

app.MapPost("/api/contact", async (AppDbContext db, ContactMessage dto) =>
{
    if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Email))
    {
        return Results.BadRequest(new { error = "name and email are required" });
    }
    dto.Id = 0;
    dto.CreatedAt = DateTime.UtcNow;
    db.ContactMessages.Add(dto);
    await db.SaveChangesAsync();
    return Results.Created($"/api/contact/{dto.Id}", dto);
});

app.MapGet("/api/contact/{id:int}", async (AppDbContext db, int id) =>
    await db.ContactMessages.FindAsync(id) is { } message ? Results.Ok(message) : Results.NotFound());

app.MapGet("/api/bookings", async (AppDbContext db) =>
    Results.Ok(await db.BookingRequests.OrderByDescending(b => b.CreatedAt).Take(50).ToListAsync()));

app.MapGet("/api/comments", async (AppDbContext db) =>
    Results.Ok(await db.BlogComments.OrderByDescending(c => c.CreatedAt).Take(50).ToListAsync()));

app.MapGet("/api/subscribers", async (AppDbContext db) =>
    Results.Ok(await db.Subscribers.OrderByDescending(s => s.CreatedAt).Take(50).ToListAsync()));

// Newsletter form in the footer posts here, then returns to the referring page.
app.MapPost("/subscribe", async (AppDbContext db, HttpContext http) =>
{
    var form = await http.Request.ReadFormAsync();
    var email = form["email"].ToString().Trim();
    if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
    {
        return Results.Redirect(RefererOrHome(http));
    }
    if (!await db.Subscribers.AnyAsync(s => s.Email == email))
    {
        db.Subscribers.Add(new Subscriber { Email = email });
        await db.SaveChangesAsync();
    }
    var url = RefererOrHome(http);
    var separator = url.Contains('?') ? '&' : '?';
    return Results.Redirect(url + separator + "subscribed=1");
});

app.MapRazorPages();

// Unknown URLs (and unknown assets) render the branded 404 page with a 404 status.
app.UseStatusCodePagesWithReExecute("/404.html");

app.Run();

static string RefererOrHome(HttpContext http)
{
    var referer = http.Request.Headers.Referer.ToString();
    if (string.IsNullOrWhiteSpace(referer)) return "/";
    // only redirect to our own site
    if (Uri.TryCreate(referer, UriKind.Absolute, out var uri) &&
        uri.Host is "localhost" or "127.0.0.1" or "web")
    {
        return uri.PathAndQuery;
    }
    return "/";
}

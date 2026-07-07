using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SafeLinkAI.Data;
using SafeLinkAI.Models;
using SafeLinkAI.Services;

var builder = WebApplication.CreateBuilder(args);

<<<<<<< HEAD
builder.Services.AddDbContext<ApplicationDbContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(o =>
{
    o.Password.RequiredLength = 6; o.Password.RequireDigit = true;
    o.Password.RequireUppercase = true; o.Password.RequireNonAlphanumeric = true;
    o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    o.Lockout.MaxFailedAccessAttempts = 5;
    o.User.RequireUniqueEmail = true;
=======
// ── Database ────────────────────────────────────────────────────────────────
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── Identity ────────────────────────────────────────────────────────────────
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password policy
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;

    // Lockout (Seguridad module)
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User
    options.User.RequireUniqueEmail = true;
>>>>>>> 7ee0d1330da358cbb81d04df34c76b99ba1f1fc3
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

<<<<<<< HEAD
builder.Services.ConfigureApplicationCookie(o =>
{
    o.LoginPath = "/Account/Login"; o.LogoutPath = "/Account/Logout";
    o.AccessDeniedPath = "/Account/AccessDenied";
    o.ExpireTimeSpan = TimeSpan.FromHours(8); o.SlidingExpiration = true;
});

=======
// ── Auth cookie ─────────────────────────────────────────────────────────────
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});

// ── App services ─────────────────────────────────────────────────────────────
>>>>>>> 7ee0d1330da358cbb81d04df34c76b99ba1f1fc3
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

<<<<<<< HEAD
if (!app.Environment.IsDevelopment()) { app.UseExceptionHandler("/Home/Error"); app.UseHsts(); }
=======
// ── Middleware ───────────────────────────────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

>>>>>>> 7ee0d1330da358cbb81d04df34c76b99ba1f1fc3
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
<<<<<<< HEAD
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

=======

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ── Seed database ─────────────────────────────────────────────────────────────
>>>>>>> 7ee0d1330da358cbb81d04df34c76b99ba1f1fc3
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
    await DbInitializer.SeedAsync(scope.ServiceProvider);
}

app.Run();

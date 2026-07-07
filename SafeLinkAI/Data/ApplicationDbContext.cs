<<<<<<< HEAD
using Microsoft.AspNetCore.Identity;
=======
>>>>>>> 7ee0d1330da358cbb81d04df34c76b99ba1f1fc3
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SafeLinkAI.Models;

namespace SafeLinkAI.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
<<<<<<< HEAD
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
=======
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

>>>>>>> 7ee0d1330da358cbb81d04df34c76b99ba1f1fc3
        public DbSet<EmergencyReport> EmergencyReports { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
<<<<<<< HEAD
            builder.Entity<EmergencyReport>()
                .HasOne(r => r.User).WithMany(u => u.Reports)
                .HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Notification>()
                .HasOne(n => n.User).WithMany()
                .HasForeignKey(n => n.UserId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Notification>()
                .HasOne(n => n.Report).WithMany()
                .HasForeignKey(n => n.ReportId).OnDelete(DeleteBehavior.SetNull);
            builder.Entity<ApplicationUser>().ToTable("Users");
        }
    }

    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var role in new[] { "Administrador", "Ciudadano" })
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));

            async Task CreateUser(string email, string name, string role, string pass)
            {
                if (await userManager.FindByEmailAsync(email) == null)
                {
                    var user = new ApplicationUser { UserName = email, Email = email, FullName = name, Role = role, EmailConfirmed = true };
                    var r = await userManager.CreateAsync(user, pass);
                    if (r.Succeeded) await userManager.AddToRoleAsync(user, role);
                }
            }
            await CreateUser("admin@safelinkai.com", "Administrador SafeLink", "Administrador", "Admin@1234!");
            await CreateUser("ciudadano@safelinkai.com", "Juan Pérez", "Ciudadano", "Citizen@1234!");
        }
    }
=======

            // EmergencyReport → User (restrict delete to avoid cascade issues)
            builder.Entity<EmergencyReport>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reports)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Notification → User
            builder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Notification → Report (optional FK, no cascade)
            builder.Entity<Notification>()
                .HasOne(n => n.Report)
                .WithMany()
                .HasForeignKey(n => n.ReportId)
                .OnDelete(DeleteBehavior.SetNull);

            // Rename Identity tables (optional, cleaner schema)
            builder.Entity<ApplicationUser>().ToTable("Users");
        }
    }
>>>>>>> 7ee0d1330da358cbb81d04df34c76b99ba1f1fc3
}

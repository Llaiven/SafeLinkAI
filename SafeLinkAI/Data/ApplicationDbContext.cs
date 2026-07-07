using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SafeLinkAI.Models;

namespace SafeLinkAI.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<EmergencyReport> EmergencyReports { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
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
}

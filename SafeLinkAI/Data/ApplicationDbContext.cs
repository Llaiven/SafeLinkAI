using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SafeLinkAI.Models;

namespace SafeLinkAI.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<EmergencyReport> EmergencyReports { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

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
}

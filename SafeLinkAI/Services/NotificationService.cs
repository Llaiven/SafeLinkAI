using Microsoft.EntityFrameworkCore;
using SafeLinkAI.Data;
using SafeLinkAI.Models;

namespace SafeLinkAI.Services
{
    public interface INotificationService
    {
        Task CreateAsync(string userId, string message, int? reportId = null);
        Task<List<Notification>> GetUserNotificationsAsync(string userId);
        Task<int> GetUnreadCountAsync(string userId);
        Task MarkAllReadAsync(string userId);
        Task DeleteAsync(int id);
    }

    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _db;

        public NotificationService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task CreateAsync(string userId, string message, int? reportId = null)
        {
            _db.Notifications.Add(new Notification
            {
                UserId = userId,
                Message = message,
                ReportId = reportId,
                CreatedAt = DateTime.Now
            });
            await _db.SaveChangesAsync();
        }

        public async Task<List<Notification>> GetUserNotificationsAsync(string userId) =>
            await _db.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(50)
                .ToListAsync();

        public async Task<int> GetUnreadCountAsync(string userId) =>
            await _db.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead);

        public async Task MarkAllReadAsync(string userId)
        {
            var notifications = await _db.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();
            notifications.ForEach(n => n.IsRead = true);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var n = await _db.Notifications.FindAsync(id);
            if (n != null)
            {
                _db.Notifications.Remove(n);
                await _db.SaveChangesAsync();
            }
        }
    }
}

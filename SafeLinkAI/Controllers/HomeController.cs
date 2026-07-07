using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SafeLinkAI.Data;
using SafeLinkAI.Models;
using SafeLinkAI.Services;
using SafeLinkAI.ViewModels;

namespace SafeLinkAI.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificationService _notificationService;

        public HomeController(
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager,
            INotificationService notificationService)
        {
            _db = db;
            _userManager = userManager;
            _notificationService = notificationService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User)!;
            var isAdmin = User.IsInRole("Administrador");

            var reportsQuery = _db.EmergencyReports.AsQueryable();
            if (!isAdmin)
                reportsQuery = reportsQuery.Where(r => r.UserId == userId);

            var vm = new DashboardViewModel
            {
                TotalReports    = await reportsQuery.CountAsync(),
                ActiveReports   = await reportsQuery.CountAsync(r => r.Status == ReportStatus.Activo),
                ResolvedReports = await reportsQuery.CountAsync(r => r.Status == ReportStatus.Resuelto),
                TotalUsers      = isAdmin ? await _db.Users.CountAsync() : 0,
                UnreadNotifications = await _notificationService.GetUnreadCountAsync(userId)
            };

            return View(vm);
        }

        // GET /Home/Notifications
        public async Task<IActionResult> Notifications()
        {
            var userId = _userManager.GetUserId(User)!;
            var notifications = await _notificationService.GetUserNotificationsAsync(userId);
            await _notificationService.MarkAllReadAsync(userId);
            return View(notifications);
        }

        // POST /Home/DeleteNotification/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            await _notificationService.DeleteAsync(id);
            return RedirectToAction(nameof(Notifications));
        }
    }
}

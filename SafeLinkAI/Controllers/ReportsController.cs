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
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificationService _notificationService;

        public ReportsController(
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager,
            INotificationService notificationService)
        {
            _db = db;
            _userManager = userManager;
            _notificationService = notificationService;
        }

        // GET /Reports
        public async Task<IActionResult> Index(ReportFilterViewModel filter)
        {
            var query = _db.EmergencyReports
                .Include(r => r.User)
                .AsQueryable();

            // Citizens only see their own reports
            if (!User.IsInRole("Administrador"))
            {
                var userId = _userManager.GetUserId(User);
                query = query.Where(r => r.UserId == userId);
            }

            if (!string.IsNullOrEmpty(filter.StatusFilter) &&
                Enum.TryParse<ReportStatus>(filter.StatusFilter, out var status))
                query = query.Where(r => r.Status == status);

            if (!string.IsNullOrEmpty(filter.CategoryFilter) &&
                Enum.TryParse<ReportCategory>(filter.CategoryFilter, out var cat))
                query = query.Where(r => r.Category == cat);

            if (!string.IsNullOrEmpty(filter.SearchTerm))
                query = query.Where(r =>
                    r.Title.Contains(filter.SearchTerm) ||
                    r.Description.Contains(filter.SearchTerm));

            var reports = await query
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            ViewBag.Filter = filter;
            return View(reports);
        }

        // GET /Reports/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var report = await _db.EmergencyReports
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (report == null) return NotFound();

            // Security: citizen can only see own reports
            if (!User.IsInRole("Administrador"))
            {
                var userId = _userManager.GetUserId(User);
                if (report.UserId != userId) return Forbid();
            }

            return View(report);
        }

        // GET /Reports/Create
        public IActionResult Create() => View(new EmergencyReport());

        // POST /Reports/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmergencyReport model)
        {
            ModelState.Remove("UserId");
            ModelState.Remove("User");

            if (!ModelState.IsValid) return View(model);

            var userId = _userManager.GetUserId(User)!;
            model.UserId = userId;
            model.CreatedAt = DateTime.Now;
            model.Status = ReportStatus.Activo;

            _db.EmergencyReports.Add(model);
            await _db.SaveChangesAsync();

            // Notify all admins
            var admins = await _userManager.GetUsersInRoleAsync("Administrador");
            foreach (var admin in admins)
            {
                await _notificationService.CreateAsync(
                    admin.Id,
                    $"Nuevo reporte: \"{model.Title}\" en categoría {model.Category}.",
                    model.Id);
            }

            TempData["Success"] = "Reporte creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET /Reports/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var report = await _db.EmergencyReports.FindAsync(id);
            if (report == null) return NotFound();

            if (!User.IsInRole("Administrador"))
            {
                var userId = _userManager.GetUserId(User);
                if (report.UserId != userId) return Forbid();
            }

            return View(report);
        }

        // POST /Reports/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EmergencyReport model)
        {
            if (id != model.Id) return BadRequest();

            ModelState.Remove("UserId");
            ModelState.Remove("User");

            if (!ModelState.IsValid) return View(model);

            var existing = await _db.EmergencyReports.FindAsync(id);
            if (existing == null) return NotFound();

            if (!User.IsInRole("Administrador"))
            {
                var userId = _userManager.GetUserId(User);
                if (existing.UserId != userId) return Forbid();
            }

            existing.Title = model.Title;
            existing.Description = model.Description;
            existing.Category = model.Category;
            existing.Address = model.Address;
            existing.Latitude = model.Latitude;
            existing.Longitude = model.Longitude;
            existing.UpdatedAt = DateTime.Now;

            // Only admin can change status
            if (User.IsInRole("Administrador"))
                existing.Status = model.Status;

            await _db.SaveChangesAsync();
            TempData["Success"] = "Reporte actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // POST /Reports/UpdateStatus/5
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, ReportStatus status)
        {
            var report = await _db.EmergencyReports.FindAsync(id);
            if (report == null) return NotFound();

            report.Status = status;
            report.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync();

            // Notify report owner
            await _notificationService.CreateAsync(
                report.UserId,
                $"Tu reporte \"{report.Title}\" cambió de estado a: {status}.",
                report.Id);

            TempData["Success"] = "Estado actualizado.";
            return RedirectToAction(nameof(Index));
        }

        // POST /Reports/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var report = await _db.EmergencyReports.FindAsync(id);
            if (report == null) return NotFound();

            if (!User.IsInRole("Administrador"))
            {
                var userId = _userManager.GetUserId(User);
                if (report.UserId != userId) return Forbid();
            }

            _db.EmergencyReports.Remove(report);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Reporte eliminado.";
            return RedirectToAction(nameof(Index));
        }
    }
}

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
        private readonly UserManager<ApplicationUser> _um;
        private readonly INotificationService _ns;
        private readonly IConfiguration _config;

        public ReportsController(ApplicationDbContext db, UserManager<ApplicationUser> um, INotificationService ns, IConfiguration config)
        { _db = db; _um = um; _ns = ns; _config = config; }

        // ── Dashboard de incidentes (NUEVO) ───────────────────────────────────
        public async Task<IActionResult> Index(ReportFilterViewModel filter)
        {
            var userId = _um.GetUserId(User)!;
            var isAdmin = User.IsInRole("Administrador");

            var query = _db.EmergencyReports.Include(r => r.User).AsQueryable();
            if (!isAdmin) query = query.Where(r => r.UserId == userId);

            if (!string.IsNullOrEmpty(filter.StatusFilter) && Enum.TryParse<ReportStatus>(filter.StatusFilter, out var s))
                query = query.Where(r => r.Status == s);
            if (!string.IsNullOrEmpty(filter.CategoryFilter) && Enum.TryParse<ReportCategory>(filter.CategoryFilter, out var c))
                query = query.Where(r => r.Category == c);
            if (!string.IsNullOrEmpty(filter.SearchTerm))
                query = query.Where(r => r.Title.Contains(filter.SearchTerm) || r.Description.Contains(filter.SearchTerm));

            var all = await query.OrderByDescending(r => r.CreatedAt).ToListAsync();

            // Stats for dashboard panel
            var allForStats = _db.EmergencyReports.AsQueryable();
            if (!isAdmin) allForStats = allForStats.Where(r => r.UserId == userId);
            var allList = await allForStats.ToListAsync();

            var vm = new ReportsDashboardViewModel
            {
                Reports = all,
                Filter = filter,
                TotalActivos    = allList.Count(r => r.Status == ReportStatus.Activo),
                TotalEnAtencion = allList.Count(r => r.Status == ReportStatus.EnAtencion),
                TotalResueltos  = allList.Count(r => r.Status == ReportStatus.Resuelto),
                PorCategoria    = allList.GroupBy(r => r.Category.ToString()).ToDictionary(g => g.Key, g => g.Count()),
                Recientes       = allList.OrderByDescending(r => r.CreatedAt).Take(5).ToList()
            };

            return View(vm);
        }

        public async Task<IActionResult> Details(int id)
        {
            var r = await _db.EmergencyReports.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == id);
            if (r == null) return NotFound();
            if (!User.IsInRole("Administrador") && r.UserId != _um.GetUserId(User)) return Forbid();
            ViewBag.MapsApiKey = _config["GoogleMaps:ApiKey"];
            return View(r);
        }

        public IActionResult Create()
        {
            ViewBag.MapsApiKey = _config["GoogleMaps:ApiKey"];
            return View(new EmergencyReport());
        }

        [HttpPost][ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmergencyReport model)
        {
            ModelState.Remove("UserId"); ModelState.Remove("User");
            if (!ModelState.IsValid) { ViewBag.MapsApiKey = _config["GoogleMaps:ApiKey"]; return View(model); }

            model.UserId = _um.GetUserId(User)!;
            model.CreatedAt = DateTime.Now;
            model.Status = ReportStatus.Activo;
            _db.EmergencyReports.Add(model);
            await _db.SaveChangesAsync();

            var admins = await _um.GetUsersInRoleAsync("Administrador");
            foreach (var a in admins)
                await _ns.CreateAsync(a.Id, $"Nuevo reporte: \"{model.Title}\" – {model.Category}", model.Id);

            TempData["Success"] = "Reporte creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var r = await _db.EmergencyReports.FindAsync(id); if (r == null) return NotFound();
            if (!User.IsInRole("Administrador") && r.UserId != _um.GetUserId(User)) return Forbid();
            ViewBag.MapsApiKey = _config["GoogleMaps:ApiKey"];
            return View(r);
        }

        [HttpPost][ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EmergencyReport model)
        {
            if (id != model.Id) return BadRequest();
            ModelState.Remove("UserId"); ModelState.Remove("User");
            if (!ModelState.IsValid) { ViewBag.MapsApiKey = _config["GoogleMaps:ApiKey"]; return View(model); }

            var existing = await _db.EmergencyReports.FindAsync(id); if (existing == null) return NotFound();
            if (!User.IsInRole("Administrador") && existing.UserId != _um.GetUserId(User)) return Forbid();

            existing.Title = model.Title; existing.Description = model.Description;
            existing.Category = model.Category; existing.Address = model.Address;
            existing.Latitude = model.Latitude; existing.Longitude = model.Longitude;
            existing.UpdatedAt = DateTime.Now;
            if (User.IsInRole("Administrador")) existing.Status = model.Status;

            await _db.SaveChangesAsync();
            TempData["Success"] = "Reporte actualizado.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost][Authorize(Roles = "Administrador")][ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, ReportStatus status)
        {
            var r = await _db.EmergencyReports.FindAsync(id); if (r == null) return NotFound();
            r.Status = status; r.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync();
            await _ns.CreateAsync(r.UserId, $"Tu reporte \"{r.Title}\" cambió a: {status}.", r.Id);
            TempData["Success"] = "Estado actualizado.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost][ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var r = await _db.EmergencyReports.FindAsync(id); if (r == null) return NotFound();
            if (!User.IsInRole("Administrador") && r.UserId != _um.GetUserId(User)) return Forbid();
            _db.EmergencyReports.Remove(r); await _db.SaveChangesAsync();
            TempData["Success"] = "Reporte eliminado.";
            return RedirectToAction(nameof(Index));
        }
    }
}

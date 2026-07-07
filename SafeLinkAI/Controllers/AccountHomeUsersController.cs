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
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _um;
        private readonly SignInManager<ApplicationUser> _sm;
        public AccountController(UserManager<ApplicationUser> um, SignInManager<ApplicationUser> sm) { _um = um; _sm = sm; }

        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true) return RedirectToAction("Index", "Home");
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost][AllowAnonymous][ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid) return View(model);
            var user = await _um.FindByEmailAsync(model.Email);
            if (user == null || !user.IsActive) { ModelState.AddModelError("", "Credenciales inválidas o cuenta desactivada."); return View(model); }
            var result = await _sm.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: true);
            if (result.Succeeded) return LocalRedirect(returnUrl ?? Url.Action("Index", "Home")!);
            if (result.IsLockedOut) ModelState.AddModelError("", "Cuenta bloqueada. Intenta en 5 minutos.");
            else ModelState.AddModelError("", "Correo o contraseña incorrectos.");
            return View(model);
        }

        [AllowAnonymous] public IActionResult Register() => View();

        [HttpPost][AllowAnonymous][ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FullName = model.FullName, PhoneNumber = model.PhoneNumber, Role = "Ciudadano" };
            var result = await _um.CreateAsync(user, model.Password);
            if (result.Succeeded) { await _um.AddToRoleAsync(user, "Ciudadano"); await _sm.SignInAsync(user, false); return RedirectToAction("Index", "Home"); }
            foreach (var e in result.Errors) ModelState.AddModelError("", e.Description);
            return View(model);
        }

        [HttpPost][ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout() { await _sm.SignOutAsync(); return RedirectToAction("Login"); }

        [AllowAnonymous] public IActionResult AccessDenied() => View();
    }

    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _um;
        private readonly INotificationService _ns;
        public HomeController(ApplicationDbContext db, UserManager<ApplicationUser> um, INotificationService ns) { _db = db; _um = um; _ns = ns; }

        public async Task<IActionResult> Index()
        {
            var userId = _um.GetUserId(User)!;
            var isAdmin = User.IsInRole("Administrador");
            var q = _db.EmergencyReports.AsQueryable();
            if (!isAdmin) q = q.Where(r => r.UserId == userId);
            var vm = new DashboardViewModel
            {
                TotalReports    = await q.CountAsync(),
                ActiveReports   = await q.CountAsync(r => r.Status == ReportStatus.Activo),
                AttendingReports = await q.CountAsync(r => r.Status == ReportStatus.EnAtencion),
                ResolvedReports = await q.CountAsync(r => r.Status == ReportStatus.Resuelto),
                TotalUsers      = isAdmin ? await _db.Users.CountAsync() : 0,
                UnreadNotifications = await _ns.GetUnreadCountAsync(userId)
            };
            return View(vm);
        }

        public async Task<IActionResult> Notifications()
        {
            var userId = _um.GetUserId(User)!;
            var n = await _ns.GetUserNotificationsAsync(userId);
            await _ns.MarkAllReadAsync(userId);
            return View(n);
        }

        [HttpPost][ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteNotification(int id) { await _ns.DeleteAsync(id); return RedirectToAction(nameof(Notifications)); }
    }

    [Authorize(Roles = "Administrador")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _um;
        public UsersController(UserManager<ApplicationUser> um) { _um = um; }

        public async Task<IActionResult> Index(string? search)
        {
            var q = _um.Users.AsQueryable();
            if (!string.IsNullOrEmpty(search)) q = q.Where(u => u.FullName.Contains(search) || u.Email!.Contains(search));
            ViewBag.Search = search;
            return View(await q.OrderByDescending(u => u.CreatedAt).ToListAsync());
        }

        public async Task<IActionResult> Details(string id) { var u = await _um.FindByIdAsync(id); if (u == null) return NotFound(); return View(u); }

        public async Task<IActionResult> Edit(string id)
        {
            var u = await _um.FindByIdAsync(id); if (u == null) return NotFound();
            return View(new EditUserViewModel { Id = u.Id, FullName = u.FullName, Email = u.Email ?? "", PhoneNumber = u.PhoneNumber, Role = u.Role, IsActive = u.IsActive });
        }

        [HttpPost][ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);
            var u = await _um.FindByIdAsync(vm.Id); if (u == null) return NotFound();
            u.FullName = vm.FullName; u.Email = vm.Email; u.UserName = vm.Email; u.PhoneNumber = vm.PhoneNumber; u.IsActive = vm.IsActive;
            if (u.Role != vm.Role) { var old = await _um.GetRolesAsync(u); await _um.RemoveFromRolesAsync(u, old); await _um.AddToRoleAsync(u, vm.Role); u.Role = vm.Role; }
            var r = await _um.UpdateAsync(u);
            if (r.Succeeded) { TempData["Success"] = "Usuario actualizado."; return RedirectToAction(nameof(Index)); }
            foreach (var e in r.Errors) ModelState.AddModelError("", e.Description);
            return View(vm);
        }

        [HttpPost][ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(string id)
        {
            var u = await _um.FindByIdAsync(id); if (u == null) return NotFound();
            u.IsActive = !u.IsActive; await _um.UpdateAsync(u);
            TempData["Success"] = u.IsActive ? $"{u.FullName} activado." : $"{u.FullName} desactivado.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost][ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var u = await _um.FindByIdAsync(id); if (u == null) return NotFound();
            var r = await _um.DeleteAsync(u);
            TempData[r.Succeeded ? "Success" : "Error"] = r.Succeeded ? "Usuario eliminado." : "No se pudo eliminar.";
            return RedirectToAction(nameof(Index));
        }
    }
}

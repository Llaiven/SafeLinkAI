using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SafeLinkAI.Models;
using SafeLinkAI.ViewModels;

namespace SafeLinkAI.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // GET /Users
        public async Task<IActionResult> Index(string? search)
        {
            var query = _userManager.Users.AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(u =>
                    u.FullName.Contains(search) ||
                    u.Email!.Contains(search));

            var users = await query
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();

            ViewBag.Search = search;
            return View(users);
        }

        // GET /Users/Details/5
        public async Task<IActionResult> Details(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }

        // GET /Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var vm = new EditUserViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? "",
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                IsActive = user.IsActive
            };
            return View(vm);
        }

        // POST /Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var user = await _userManager.FindByIdAsync(vm.Id);
            if (user == null) return NotFound();

            user.FullName = vm.FullName;
            user.Email = vm.Email;
            user.UserName = vm.Email;
            user.PhoneNumber = vm.PhoneNumber;
            user.IsActive = vm.IsActive;

            // Sync role
            if (user.Role != vm.Role)
            {
                var oldRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, oldRoles);
                await _userManager.AddToRoleAsync(user, vm.Role);
                user.Role = vm.Role;
            }

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = "Usuario actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(vm);
        }

        // POST /Users/ToggleActive/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            user.IsActive = !user.IsActive;
            await _userManager.UpdateAsync(user);

            TempData["Success"] = user.IsActive
                ? $"Usuario {user.FullName} activado."
                : $"Usuario {user.FullName} desactivado.";

            return RedirectToAction(nameof(Index));
        }

        // POST /Users/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
                TempData["Success"] = "Usuario eliminado correctamente.";
            else
                TempData["Error"] = "No se pudo eliminar el usuario.";

            return RedirectToAction(nameof(Index));
        }
    }
}

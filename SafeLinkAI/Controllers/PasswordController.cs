using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SafeLinkAI.Models;
using SafeLinkAI.ViewModels;

namespace SafeLinkAI.Controllers
{
    [AllowAnonymous]
    public class PasswordController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public PasswordController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // ── PASO 1: Ingresar correo ───────────────────────────────────────────
        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            // Siempre mostramos la misma pantalla para no revelar si el correo existe
            if (user == null || !user.IsActive)
                return RedirectToAction(nameof(ForgotPasswordConfirmation));

            // Generar token de restablecimiento
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // En producción esto se enviaría por email.
            // Para el contexto académico, redirigimos directamente al reset con el token.
            return RedirectToAction(nameof(ResetPassword), new { token, email = user.Email });
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation() => View();

        // ── PASO 2: Nueva contraseña ─────────────────────────────────────────
        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
                return BadRequest("Token o correo inválido.");

            return View(new ResetPasswordViewModel { Token = token, Email = email });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return RedirectToAction(nameof(ResetPasswordConfirmation));

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

            if (result.Succeeded)
                return RedirectToAction(nameof(ResetPasswordConfirmation));

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation() => View();
    }
}

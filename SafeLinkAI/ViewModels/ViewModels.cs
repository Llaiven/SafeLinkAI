using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using SafeLinkAI.Data;
using SafeLinkAI.Models;

namespace SafeLinkAI.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress] [Display(Name = "Correo")] public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)] [Display(Name = "Contraseña")] public string Password { get; set; } = string.Empty;
        [Display(Name = "Recordarme")] public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required][StringLength(100)][Display(Name = "Nombre Completo")] public string FullName { get; set; } = string.Empty;
        [Required][EmailAddress][Display(Name = "Correo")] public string Email { get; set; } = string.Empty;
        [Phone][Display(Name = "Teléfono")] public string? PhoneNumber { get; set; }
        [Required][StringLength(100, MinimumLength = 6)][DataType(DataType.Password)][Display(Name = "Contraseña")] public string Password { get; set; } = string.Empty;
        [DataType(DataType.Password)][Display(Name = "Confirmar contraseña")][Compare("Password", ErrorMessage = "Las contraseñas no coinciden")] public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class EditUserViewModel
    {
        public string Id { get; set; } = string.Empty;
        [Required][StringLength(100)][Display(Name = "Nombre Completo")] public string FullName { get; set; } = string.Empty;
        [Required][EmailAddress][Display(Name = "Correo")] public string Email { get; set; } = string.Empty;
        [Phone][Display(Name = "Teléfono")] public string? PhoneNumber { get; set; }
        [Display(Name = "Rol")] public string Role { get; set; } = "Ciudadano";
        [Display(Name = "¿Activo?")] public bool IsActive { get; set; }
    }

    public class ReportFilterViewModel
    {
        public string? StatusFilter { get; set; }
        public string? CategoryFilter { get; set; }
        public string? SearchTerm { get; set; }
    }

    public class DashboardViewModel
    {
        public int TotalReports { get; set; }
        public int ActiveReports { get; set; }
        public int AttendingReports { get; set; }
        public int ResolvedReports { get; set; }
        public int TotalUsers { get; set; }
        public int UnreadNotifications { get; set; }
    }

    // Dashboard de reportes por categoría
    public class ReportsDashboardViewModel
    {
        public List<EmergencyReport> Reports { get; set; } = new();
        public ReportFilterViewModel Filter { get; set; } = new();
        public int TotalActivos { get; set; }
        public int TotalEnAtencion { get; set; }
        public int TotalResueltos { get; set; }
        public Dictionary<string, int> PorCategoria { get; set; } = new();
        public List<EmergencyReport> Recientes { get; set; } = new();
    }
}

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
        public NotificationService(ApplicationDbContext db) { _db = db; }

        public async Task CreateAsync(string userId, string message, int? reportId = null)
        {
            _db.Notifications.Add(new Notification { UserId = userId, Message = message, ReportId = reportId });
            await _db.SaveChangesAsync();
        }
        public async Task<List<Notification>> GetUserNotificationsAsync(string userId) =>
            await _db.Notifications.Where(n => n.UserId == userId).OrderByDescending(n => n.CreatedAt).Take(50).ToListAsync();
        public async Task<int> GetUnreadCountAsync(string userId) =>
            await _db.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead);
        public async Task MarkAllReadAsync(string userId)
        {
            var list = await _db.Notifications.Where(n => n.UserId == userId && !n.IsRead).ToListAsync();
            list.ForEach(n => n.IsRead = true);
            await _db.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var n = await _db.Notifications.FindAsync(id);
            if (n != null) { _db.Notifications.Remove(n); await _db.SaveChangesAsync(); }
        }
    }
}

// ─── Password Recovery ────────────────────────────────────────────────────────
namespace SafeLinkAI.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Correo inválido")]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; } = string.Empty;
    }

    public class ResetPasswordViewModel
    {
        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La nueva contraseña es obligatoria")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mínimo 6 caracteres")]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva contraseña")]
        public string NewPassword { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}

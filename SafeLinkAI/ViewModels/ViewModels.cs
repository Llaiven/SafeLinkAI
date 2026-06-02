using System.ComponentModel.DataAnnotations;

namespace SafeLinkAI.ViewModels
{
    // ─── Auth ────────────────────────────────────────────────────────────────

    public class LoginViewModel
    {
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Correo inválido")]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Recordarme")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "El nombre completo es obligatorio")]
        [StringLength(100)]
        [Display(Name = "Nombre Completo")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Correo inválido")]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [Display(Name = "Teléfono")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mínimo 6 caracteres")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    // ─── Users ───────────────────────────────────────────────────────────────

    public class EditUserViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre completo es obligatorio")]
        [StringLength(100)]
        [Display(Name = "Nombre Completo")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [Display(Name = "Teléfono")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Rol")]
        public string Role { get; set; } = "Ciudadano";

        [Display(Name = "¿Activo?")]
        public bool IsActive { get; set; }
    }

    // ─── Reports ─────────────────────────────────────────────────────────────

    public class ReportFilterViewModel
    {
        public string? StatusFilter { get; set; }
        public string? CategoryFilter { get; set; }
        public string? SearchTerm { get; set; }
    }

    // ─── Dashboard ───────────────────────────────────────────────────────────

    public class DashboardViewModel
    {
        public int TotalReports { get; set; }
        public int ActiveReports { get; set; }
        public int ResolvedReports { get; set; }
        public int TotalUsers { get; set; }
        public int UnreadNotifications { get; set; }
    }
}

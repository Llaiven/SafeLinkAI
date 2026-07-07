using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SafeLinkAI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = "Ciudadano";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
        public ICollection<EmergencyReport> Reports { get; set; } = new List<EmergencyReport>();
    }

    public enum ReportCategory { Accidente, Incendio, Robo, Inundacion, Otro }
    public enum ReportStatus { Activo, EnAtencion, Resuelto }

    public class EmergencyReport
    {
        [Key] public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio")]
        [StringLength(150)]
        [Display(Name = "Título")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(1000)]
        [Display(Name = "Descripción")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Categoría")]
        public ReportCategory Category { get; set; }

        [Display(Name = "Estado")]
        public ReportStatus Status { get; set; } = ReportStatus.Activo;

        [StringLength(200)]
        [Display(Name = "Dirección")]
        public string? Address { get; set; }

        [Display(Name = "Latitud")]
        public double? Latitude { get; set; }

        [Display(Name = "Longitud")]
        public double? Longitude { get; set; }

        [Display(Name = "Fecha de Reporte")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public string UserId { get; set; } = string.Empty;
        [ForeignKey("UserId")] public ApplicationUser? User { get; set; }
    }

    public class Notification
    {
        [Key] public int Id { get; set; }
        [Required][StringLength(200)] public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string UserId { get; set; } = string.Empty;
        [ForeignKey("UserId")] public ApplicationUser? User { get; set; }
        public int? ReportId { get; set; }
        [ForeignKey("ReportId")] public EmergencyReport? Report { get; set; }
    }
}

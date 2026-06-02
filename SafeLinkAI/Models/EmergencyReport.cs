using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SafeLinkAI.Models
{
    public enum ReportCategory
    {
        Accidente,
        Incendio,
        Robo,
        Inundacion,
        Otro
    }

    public enum ReportStatus
    {
        Activo,
        EnAtencion,
        Resuelto
    }

    public class EmergencyReport
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio")]
        [StringLength(150, ErrorMessage = "Máximo 150 caracteres")]
        [Display(Name = "Título")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(1000)]
        [Display(Name = "Descripción")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "La categoría es obligatoria")]
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

        [Display(Name = "Última Actualización")]
        public DateTime? UpdatedAt { get; set; }

        // FK
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }
    }
}

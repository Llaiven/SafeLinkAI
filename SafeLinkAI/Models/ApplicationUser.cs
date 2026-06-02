using Microsoft.AspNetCore.Identity;

namespace SafeLinkAI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string Role { get; set; } = "Ciudadano"; // Ciudadano | Administrador
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;

        // Navigation
        public ICollection<EmergencyReport> Reports { get; set; } = new List<EmergencyReport>();
    }
}

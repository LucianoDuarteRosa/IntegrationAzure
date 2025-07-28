using System.ComponentModel.DataAnnotations;

namespace IntegrationAzure.Api.Domain.Entities
{
    public class Configuration
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Key { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string Value { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Category { get; set; } = string.Empty;

        public bool IsSecret { get; set; } = false;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(100)]
        public string CreatedBy { get; set; } = string.Empty;

        [MaxLength(100)]
        public string UpdatedBy { get; set; } = string.Empty;
    }
}

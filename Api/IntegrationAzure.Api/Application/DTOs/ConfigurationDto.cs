using System.ComponentModel.DataAnnotations;

namespace IntegrationAzure.Api.Application.DTOs
{
    public class ConfigurationDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "A chave é obrigatória")]
        [MaxLength(100, ErrorMessage = "A chave deve ter no máximo 100 caracteres")]
        public string Key { get; set; } = string.Empty;

        [Required(ErrorMessage = "O valor é obrigatório")]
        [MaxLength(1000, ErrorMessage = "O valor deve ter no máximo 1000 caracteres")]
        public string Value { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres")]
        public string Description { get; set; } = string.Empty;

        [MaxLength(50, ErrorMessage = "A categoria deve ter no máximo 50 caracteres")]
        public string Category { get; set; } = string.Empty;

        public bool IsSecret { get; set; } = false;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public string CreatedBy { get; set; } = string.Empty;

        public string UpdatedBy { get; set; } = string.Empty;
    }

    public class CreateConfigurationDto
    {
        [Required(ErrorMessage = "A chave é obrigatória")]
        [MaxLength(100, ErrorMessage = "A chave deve ter no máximo 100 caracteres")]
        public string Key { get; set; } = string.Empty;

        [Required(ErrorMessage = "O valor é obrigatório")]
        [MaxLength(1000, ErrorMessage = "O valor deve ter no máximo 1000 caracteres")]
        public string Value { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres")]
        public string Description { get; set; } = string.Empty;

        [MaxLength(50, ErrorMessage = "A categoria deve ter no máximo 50 caracteres")]
        public string Category { get; set; } = string.Empty;

        public bool IsSecret { get; set; } = false;

        public bool IsActive { get; set; } = true;
    }

    public class UpdateConfigurationDto
    {
        [Required(ErrorMessage = "O valor é obrigatório")]
        [MaxLength(1000, ErrorMessage = "O valor deve ter no máximo 1000 caracteres")]
        public string Value { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres")]
        public string Description { get; set; } = string.Empty;

        [MaxLength(50, ErrorMessage = "A categoria deve ter no máximo 50 caracteres")]
        public string Category { get; set; } = string.Empty;

        public bool IsSecret { get; set; } = false;

        public bool IsActive { get; set; } = true;
    }
}

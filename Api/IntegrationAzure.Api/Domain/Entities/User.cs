using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntegrationAzure.Api.Domain.Entities;

/// <summary>
/// Entidade que representa os usuários do sistema
/// Inclui informações pessoais, credenciais e perfil de acesso
/// </summary>
public class User : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Nickname { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Password { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? ProfileImagePath { get; set; }

    // Chave estrangeira para Profile
    [Required]
    public Guid ProfileId { get; set; }

    // Relacionamento com Profile
    [ForeignKey("ProfileId")]
    public virtual Profile Profile { get; set; } = null!;
}

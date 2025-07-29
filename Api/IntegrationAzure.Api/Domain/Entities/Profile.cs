using System.ComponentModel.DataAnnotations;

namespace IntegrationAzure.Api.Domain.Entities;

/// <summary>
/// Entidade que representa os perfis de usuários no sistema
/// Define os níveis de acesso: Desenvolvedor, Administrador e Usuário
/// </summary>
public class Profile : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Description { get; set; }

    // Relacionamento com usuários
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}

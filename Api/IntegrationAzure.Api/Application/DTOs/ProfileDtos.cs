using System.ComponentModel.DataAnnotations;

namespace IntegrationAzure.Api.Application.DTOs;

/// <summary>
/// DTO para criação de perfil
/// </summary>
public class CreateProfileDto
{
    [Required(ErrorMessage = "O nome é obrigatório")]
    [StringLength(50, ErrorMessage = "O nome deve ter no máximo 50 caracteres")]
    public string Name { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "A descrição deve ter no máximo 200 caracteres")]
    public string? Description { get; set; }
}

/// <summary>
/// DTO para atualização de perfil
/// </summary>
public class UpdateProfileDto
{
    [Required(ErrorMessage = "O nome é obrigatório")]
    [StringLength(50, ErrorMessage = "O nome deve ter no máximo 50 caracteres")]
    public string Name { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "A descrição deve ter no máximo 200 caracteres")]
    public string? Description { get; set; }
}

/// <summary>
/// DTO para retorno de perfil
/// </summary>
public class ProfileDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
    public bool IsActive { get; set; }
    public int UsersCount { get; set; }
}

/// <summary>
/// DTO simplificado para retorno de perfil em listas
/// </summary>
public class SimpleProfileDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

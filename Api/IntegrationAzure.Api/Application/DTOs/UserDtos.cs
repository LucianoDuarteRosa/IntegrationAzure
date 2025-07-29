using System.ComponentModel.DataAnnotations;

namespace IntegrationAzure.Api.Application.DTOs;

/// <summary>
/// DTO para criação de usuário
/// </summary>
public class CreateUserDto
{
    [Required(ErrorMessage = "O nome é obrigatório")]
    [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "O nickname é obrigatório")]
    [StringLength(50, ErrorMessage = "O nickname deve ter no máximo 50 caracteres")]
    public string Nickname { get; set; } = string.Empty;

    [Required(ErrorMessage = "O email é obrigatório")]
    [EmailAddress(ErrorMessage = "Formato de email inválido")]
    [StringLength(255, ErrorMessage = "O email deve ter no máximo 255 caracteres")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória")]
    [StringLength(255, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 255 caracteres")]
    public string Password { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "O caminho da imagem deve ter no máximo 500 caracteres")]
    public string? ProfileImagePath { get; set; }

    [Required(ErrorMessage = "O perfil é obrigatório")]
    public Guid ProfileId { get; set; }
}

/// <summary>
/// DTO para atualização de usuário
/// </summary>
public class UpdateUserDto
{
    [Required(ErrorMessage = "O nome é obrigatório")]
    [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "O nickname é obrigatório")]
    [StringLength(50, ErrorMessage = "O nickname deve ter no máximo 50 caracteres")]
    public string Nickname { get; set; } = string.Empty;

    [Required(ErrorMessage = "O email é obrigatório")]
    [EmailAddress(ErrorMessage = "Formato de email inválido")]
    [StringLength(255, ErrorMessage = "O email deve ter no máximo 255 caracteres")]
    public string Email { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "O caminho da imagem deve ter no máximo 500 caracteres")]
    public string? ProfileImagePath { get; set; }

    [Required(ErrorMessage = "O perfil é obrigatório")]
    public Guid ProfileId { get; set; }
}

/// <summary>
/// DTO para alteração de senha
/// </summary>
public class ChangePasswordDto
{
    [Required(ErrorMessage = "A senha atual é obrigatória")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "A nova senha é obrigatória")]
    [StringLength(255, MinimumLength = 6, ErrorMessage = "A nova senha deve ter entre 6 e 255 caracteres")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "A confirmação de senha é obrigatória")]
    [Compare("NewPassword", ErrorMessage = "A confirmação de senha não confere")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

/// <summary>
/// DTO para retorno de usuário
/// </summary>
public class UserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? ProfileImagePath { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
    public bool IsActive { get; set; }
    public SimpleProfileDto Profile { get; set; } = null!;
}

/// <summary>
/// DTO simplificado para retorno de usuário em listas
/// </summary>
public class SimpleUserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? ProfileImagePath { get; set; }
    public string ProfileName { get; set; } = string.Empty;
}

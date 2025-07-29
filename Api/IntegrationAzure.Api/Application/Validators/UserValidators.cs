using FluentValidation;
using IntegrationAzure.Api.Application.DTOs;

namespace IntegrationAzure.Api.Application.Validators;

/// <summary>
/// Validador para CreateUserDto
/// </summary>
public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("O nome é obrigatório")
            .MaximumLength(100)
            .WithMessage("O nome deve ter no máximo 100 caracteres")
            .Matches(@"^[a-zA-ZÀ-ÿ\s]+$")
            .WithMessage("O nome deve conter apenas letras e espaços");

        RuleFor(x => x.Nickname)
            .NotEmpty()
            .WithMessage("O nickname é obrigatório")
            .MaximumLength(50)
            .WithMessage("O nickname deve ter no máximo 50 caracteres")
            .Matches(@"^[a-zA-Z0-9_.-]+$")
            .WithMessage("O nickname deve conter apenas letras, números, pontos, hífens e sublinhados");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("O email é obrigatório")
            .EmailAddress()
            .WithMessage("Formato de email inválido")
            .MaximumLength(255)
            .WithMessage("O email deve ter no máximo 255 caracteres");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("A senha é obrigatória")
            .MinimumLength(6)
            .WithMessage("A senha deve ter pelo menos 6 caracteres")
            .MaximumLength(255)
            .WithMessage("A senha deve ter no máximo 255 caracteres")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).*$")
            .WithMessage("A senha deve conter pelo menos uma letra minúscula, uma maiúscula e um número");

        RuleFor(x => x.ProfileImagePath)
            .MaximumLength(500)
            .WithMessage("O caminho da imagem deve ter no máximo 500 caracteres")
            .When(x => !string.IsNullOrEmpty(x.ProfileImagePath));

        RuleFor(x => x.ProfileId)
            .NotEmpty()
            .WithMessage("O perfil é obrigatório");
    }
}

/// <summary>
/// Validador para UpdateUserDto
/// </summary>
public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("O nome é obrigatório")
            .MaximumLength(100)
            .WithMessage("O nome deve ter no máximo 100 caracteres")
            .Matches(@"^[a-zA-ZÀ-ÿ\s]+$")
            .WithMessage("O nome deve conter apenas letras e espaços");

        RuleFor(x => x.Nickname)
            .NotEmpty()
            .WithMessage("O nickname é obrigatório")
            .MaximumLength(50)
            .WithMessage("O nickname deve ter no máximo 50 caracteres")
            .Matches(@"^[a-zA-Z0-9_.-]+$")
            .WithMessage("O nickname deve conter apenas letras, números, pontos, hífens e sublinhados");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("O email é obrigatório")
            .EmailAddress()
            .WithMessage("Formato de email inválido")
            .MaximumLength(255)
            .WithMessage("O email deve ter no máximo 255 caracteres");

        RuleFor(x => x.ProfileImagePath)
            .MaximumLength(500)
            .WithMessage("O caminho da imagem deve ter no máximo 500 caracteres")
            .When(x => !string.IsNullOrEmpty(x.ProfileImagePath));

        RuleFor(x => x.ProfileId)
            .NotEmpty()
            .WithMessage("O perfil é obrigatório");
    }
}

/// <summary>
/// Validador para ChangePasswordDto
/// </summary>
public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
{
    public ChangePasswordDtoValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty()
            .WithMessage("A senha atual é obrigatória");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage("A nova senha é obrigatória")
            .MinimumLength(6)
            .WithMessage("A nova senha deve ter pelo menos 6 caracteres")
            .MaximumLength(255)
            .WithMessage("A nova senha deve ter no máximo 255 caracteres")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).*$")
            .WithMessage("A nova senha deve conter pelo menos uma letra minúscula, uma maiúscula e um número");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .WithMessage("A confirmação de senha é obrigatória")
            .Equal(x => x.NewPassword)
            .WithMessage("A confirmação de senha não confere");
    }
}

using FluentValidation;
using IntegrationAzure.Api.Application.DTOs;

namespace IntegrationAzure.Api.Application.Validators;

/// <summary>
/// Validador para CreateProfileDto
/// </summary>
public class CreateProfileDtoValidator : AbstractValidator<CreateProfileDto>
{
    public CreateProfileDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("O nome é obrigatório")
            .MaximumLength(50)
            .WithMessage("O nome deve ter no máximo 50 caracteres")
            .Matches(@"^[a-zA-ZÀ-ÿ\s]+$")
            .WithMessage("O nome deve conter apenas letras e espaços");

        RuleFor(x => x.Description)
            .MaximumLength(200)
            .WithMessage("A descrição deve ter no máximo 200 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}

/// <summary>
/// Validador para UpdateProfileDto
/// </summary>
public class UpdateProfileDtoValidator : AbstractValidator<UpdateProfileDto>
{
    public UpdateProfileDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("O nome é obrigatório")
            .MaximumLength(50)
            .WithMessage("O nome deve ter no máximo 50 caracteres")
            .Matches(@"^[a-zA-ZÀ-ÿ\s]+$")
            .WithMessage("O nome deve conter apenas letras e espaços");

        RuleFor(x => x.Description)
            .MaximumLength(200)
            .WithMessage("A descrição deve ter no máximo 200 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}

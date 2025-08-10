using FluentValidation;
using IntegrationAzure.Api.Application.DTOs;

namespace IntegrationAzure.Api.Application.Validators;

/// <summary>
/// Validador para criação de issues
/// </summary>
public class CreateIssueDtoValidator : AbstractValidator<CreateIssueDto>
{
    public CreateIssueDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Título é obrigatório")
            .MaximumLength(255)
            .WithMessage("Título deve ter no máximo 255 caracteres")
            .MinimumLength(5)
            .WithMessage("Título deve ter pelo menos 5 caracteres");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Descrição é obrigatória")
            .MinimumLength(5)
            .WithMessage("Descrição deve ter pelo menos 5 caracteres");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Tipo da issue deve ser um valor válido");

        RuleFor(x => x.Priority)
            .IsInEnum()
            .WithMessage("Prioridade deve ser um valor válido");

        RuleFor(x => x.Activity)
            .MaximumLength(100)
            .WithMessage("Activity deve ter no máximo 100 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Activity));

        RuleFor(x => x.Environment)
            .MaximumLength(200)
            .WithMessage("Ambiente deve ter no máximo 200 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Environment));

        // Validação para UserStoryId - obrigatório
        RuleFor(x => x.UserStoryId)
            .NotEmpty()
            .WithMessage("User Story é obrigatória")
            .GreaterThan(0)
            .WithMessage("UserStoryId deve ser um número positivo");
    }
}
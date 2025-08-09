using FluentValidation;
using IntegrationAzure.Api.Application.DTOs;
using IntegrationAzure.Api.Domain.Entities;

namespace IntegrationAzure.Api.Application.Validators;

/// <summary>
/// Validador para criação de falhas
/// </summary>
public class CreateFailureDtoValidator : AbstractValidator<CreateFailureDto>
{
    public CreateFailureDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Título é obrigatório")
            .MaximumLength(255)
            .WithMessage("Título deve ter no máximo 255 caracteres")
            .MinimumLength(5)
            .WithMessage("Título deve ter pelo menos 5 caracteres");

        RuleFor(x => x.Severity)
            .IsInEnum()
            .WithMessage("Severidade deve ser um valor válido");

        RuleFor(x => x.Activity)
            .NotEmpty()
            .WithMessage("Atividade é obrigatória");

        RuleFor(x => x.OccurredAt)
            .NotEmpty()
            .WithMessage("Data de ocorrência é obrigatória")
            .LessThanOrEqualTo(DateTime.UtcNow.AddMinutes(5))
            .WithMessage("Data de ocorrência não pode ser no futuro");

        RuleFor(x => x.Environment)
            .MaximumLength(200)
            .WithMessage("Ambiente deve ter no máximo 200 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Environment));

        // Validação para UserStoryId - deve ser um inteiro válido se fornecido
        RuleFor(x => x.UserStoryId)
            .GreaterThan(0)
            .WithMessage("UserStoryId deve ser um número positivo")
            .When(x => x.UserStoryId.HasValue);

        // Validação para cenários estruturados - pelo menos um cenário completo é obrigatório
        RuleFor(x => x.Scenarios)
            .Must(scenarios => scenarios != null && scenarios.Any())
            .WithMessage("Pelo menos um cenário (Dado que/Quando/Então) é obrigatório");

        RuleForEach(x => x.Scenarios)
            .ChildRules(scenario =>
            {
                scenario.RuleFor(s => s.Given)
                    .NotEmpty()
                    .WithMessage("'Processo Atual' é obrigatório");

                scenario.RuleFor(s => s.When)
                    .NotEmpty()
                    .WithMessage("'Ação Executada' é obrigatória");

                scenario.RuleFor(s => s.Then)
                    .NotEmpty()
                    .WithMessage("'Melhoria Esperada' é obrigatória");
            })
            .When(x => x.Scenarios != null && x.Scenarios.Any());
    }
}

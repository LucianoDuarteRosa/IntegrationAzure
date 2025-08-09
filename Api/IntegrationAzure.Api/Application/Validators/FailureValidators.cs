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
        RuleFor(x => x.FailureNumber)
            .NotEmpty()
            .WithMessage("Número da falha é obrigatório")
            .MaximumLength(50)
            .WithMessage("Número da falha deve ter no máximo 50 caracteres")
            .Matches(@"^(FLH|INC|OUT)-\d{3,6}$")
            .WithMessage("Número da falha deve seguir o formato XXX-000 (ex: FLH-001, INC-001)");

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
            .MinimumLength(10)
            .WithMessage("Descrição deve ter pelo menos 10 caracteres")
            .When(x => x.Scenarios == null || !x.Scenarios.Any()); // Só exige descrição se não há cenários estruturados

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

        // Validação para cenários estruturados
        RuleFor(x => x.Scenarios)
            .Must(scenarios => scenarios == null || scenarios.Any())
            .WithMessage("Pelo menos um cenário deve ser informado quando usar cenários estruturados")
            .When(x => string.IsNullOrEmpty(x.Description));

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

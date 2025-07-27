using FluentValidation;
using IntegrationAzure.Api.Application.DTOs;

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
            .WithMessage("Descrição deve ter pelo menos 10 caracteres");

        RuleFor(x => x.Severity)
            .IsInEnum()
            .WithMessage("Severidade deve ser um valor válido");

        RuleFor(x => x.OccurredAt)
            .NotEmpty()
            .WithMessage("Data de ocorrência é obrigatória")
            .LessThanOrEqualTo(DateTime.UtcNow.AddMinutes(5))
            .WithMessage("Data de ocorrência não pode ser no futuro");

        RuleFor(x => x.ReportedBy)
            .MaximumLength(100)
            .WithMessage("Relator deve ter no máximo 100 caracteres")
            .When(x => !string.IsNullOrEmpty(x.ReportedBy));

        RuleFor(x => x.AssignedTo)
            .MaximumLength(100)
            .WithMessage("Responsável deve ter no máximo 100 caracteres")
            .When(x => !string.IsNullOrEmpty(x.AssignedTo));

        RuleFor(x => x.Environment)
            .MaximumLength(200)
            .WithMessage("Ambiente deve ter no máximo 200 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Environment));

        RuleFor(x => x.EstimatedImpactCost)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Custo estimado deve ser maior ou igual a zero")
            .When(x => x.EstimatedImpactCost.HasValue);
    }
}

/// <summary>
/// Validador para atualização de falhas
/// </summary>
public class UpdateFailureDtoValidator : AbstractValidator<UpdateFailureDto>
{
    public UpdateFailureDtoValidator()
    {
        RuleFor(x => x.Title)
            .MaximumLength(255)
            .WithMessage("Título deve ter no máximo 255 caracteres")
            .MinimumLength(5)
            .WithMessage("Título deve ter pelo menos 5 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Title));

        RuleFor(x => x.Description)
            .MinimumLength(10)
            .WithMessage("Descrição deve ter pelo menos 10 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Severity)
            .IsInEnum()
            .WithMessage("Severidade deve ser um valor válido")
            .When(x => x.Severity.HasValue);

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Status deve ser um valor válido")
            .When(x => x.Status.HasValue);

        RuleFor(x => x.AssignedTo)
            .MaximumLength(100)
            .WithMessage("Responsável deve ter no máximo 100 caracteres")
            .When(x => !string.IsNullOrEmpty(x.AssignedTo));

        RuleFor(x => x.Environment)
            .MaximumLength(200)
            .WithMessage("Ambiente deve ter no máximo 200 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Environment));

        RuleFor(x => x.EstimatedImpactCost)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Custo estimado deve ser maior ou igual a zero")
            .When(x => x.EstimatedImpactCost.HasValue);

        RuleFor(x => x.DowntimeDuration)
            .GreaterThanOrEqualTo(TimeSpan.Zero)
            .WithMessage("Duração do downtime deve ser maior ou igual a zero")
            .When(x => x.DowntimeDuration.HasValue);
    }
}

/// <summary>
/// Validador para criação de casos de teste
/// </summary>
public class CreateTestCaseDtoValidator : AbstractValidator<CreateTestCaseDto>
{
    public CreateTestCaseDtoValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Descrição do caso de teste é obrigatória")
            .MinimumLength(5)
            .WithMessage("Descrição deve ter pelo menos 5 caracteres")
            .MaximumLength(500)
            .WithMessage("Descrição deve ter no máximo 500 caracteres");

        RuleFor(x => x.OrderIndex)
            .GreaterThan(0)
            .WithMessage("Índice de ordem deve ser maior que zero");
    }
}

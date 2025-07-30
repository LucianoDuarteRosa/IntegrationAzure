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
        RuleFor(x => x.IssueNumber)
            .NotEmpty()
            .WithMessage("Número da issue é obrigatório")
            .MaximumLength(50)
            .WithMessage("Número da issue deve ter no máximo 50 caracteres")
            .Matches(@"^(ISS|BUG|FEA|TSK)-\d{3,6}$")
            .WithMessage("Número da issue deve seguir o formato XXX-000 (ex: ISS-001, BUG-001)");

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

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Tipo da issue deve ser um valor válido");

        RuleFor(x => x.Priority)
            .IsInEnum()
            .WithMessage("Prioridade deve ser um valor válido");

        RuleFor(x => x.OccurrenceType)
            .GreaterThan(0)
            .WithMessage("Tipo de ocorrência deve ser um valor válido");

        RuleFor(x => x.Environment)
            .MaximumLength(200)
            .WithMessage("Ambiente deve ter no máximo 200 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Environment));
    }
}

/// <summary>
/// Validador para atualização de issues
/// </summary>
public class UpdateIssueDtoValidator : AbstractValidator<UpdateIssueDto>
{
    public UpdateIssueDtoValidator()
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

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Tipo da issue deve ser um valor válido")
            .When(x => x.Type.HasValue);

        RuleFor(x => x.Priority)
            .IsInEnum()
            .WithMessage("Prioridade deve ser um valor válido")
            .When(x => x.Priority.HasValue);

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Status deve ser um valor válido")
            .When(x => x.Status.HasValue);

        RuleFor(x => x.Environment)
            .MaximumLength(200)
            .WithMessage("Ambiente deve ter no máximo 200 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Environment));
    }
}

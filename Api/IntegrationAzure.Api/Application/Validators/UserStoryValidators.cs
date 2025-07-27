using FluentValidation;
using IntegrationAzure.Api.Application.DTOs;

namespace IntegrationAzure.Api.Application.Validators;

/// <summary>
/// Validador para criação de histórias de usuário
/// Aplica regras de negócio conforme especificado no copilot.json
/// </summary>
public class CreateUserStoryDtoValidator : AbstractValidator<CreateUserStoryDto>
{
    public CreateUserStoryDtoValidator()
    {
        RuleFor(x => x.DemandNumber)
            .NotEmpty()
            .WithMessage("Número da demanda é obrigatório")
            .MaximumLength(50)
            .WithMessage("Número da demanda deve ter no máximo 50 caracteres")
            .Matches(@"^[A-Z]{2,4}-\d{3,6}$")
            .WithMessage("Número da demanda deve seguir o formato XXX-000 (ex: DEM-001)");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Título é obrigatório")
            .MaximumLength(255)
            .WithMessage("Título deve ter no máximo 255 caracteres")
            .MinimumLength(5)
            .WithMessage("Título deve ter pelo menos 5 caracteres");

        RuleFor(x => x.AcceptanceCriteria)
            .NotEmpty()
            .WithMessage("Critérios de aceite são obrigatórios")
            .MinimumLength(10)
            .WithMessage("Critérios de aceite devem ter pelo menos 10 caracteres");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Descrição deve ter no máximo 1000 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Priority)
            .IsInEnum()
            .WithMessage("Prioridade deve ser um valor válido");

        RuleFor(x => x.TestCases)
            .NotNull()
            .WithMessage("Lista de casos de teste não pode ser nula")
            .Must(x => x.Count > 0)
            .WithMessage("Deve haver pelo menos um caso de teste");

        RuleForEach(x => x.TestCases)
            .SetValidator(new CreateTestCaseDtoValidator());
    }
}

/// <summary>
/// Validador para atualização de histórias de usuário
/// </summary>
public class UpdateUserStoryDtoValidator : AbstractValidator<UpdateUserStoryDto>
{
    public UpdateUserStoryDtoValidator()
    {
        RuleFor(x => x.Title)
            .MaximumLength(255)
            .WithMessage("Título deve ter no máximo 255 caracteres")
            .MinimumLength(5)
            .WithMessage("Título deve ter pelo menos 5 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Title));

        RuleFor(x => x.AcceptanceCriteria)
            .MinimumLength(10)
            .WithMessage("Critérios de aceite devem ter pelo menos 10 caracteres")
            .When(x => !string.IsNullOrEmpty(x.AcceptanceCriteria));

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Descrição deve ter no máximo 1000 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Status deve ser um valor válido")
            .When(x => x.Status.HasValue);

        RuleFor(x => x.Priority)
            .IsInEnum()
            .WithMessage("Prioridade deve ser um valor válido")
            .When(x => x.Priority.HasValue);

        RuleForEach(x => x.TestCases)
            .SetValidator(new CreateTestCaseDtoValidator())
            .When(x => x.TestCases != null);
    }
}

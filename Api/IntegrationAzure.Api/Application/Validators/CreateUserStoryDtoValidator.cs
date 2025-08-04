using FluentValidation;
using IntegrationAzure.Api.Application.DTOs;

namespace IntegrationAzure.Api.Application.Validators;

/// <summary>
/// Validador para criação de histórias de usuário com dados estruturados
/// </summary>
public class CreateUserStoryDtoValidator : AbstractValidator<CreateUserStoryDto>
{
    public CreateUserStoryDtoValidator()
    {
        RuleFor(x => x.DemandNumber)
            .NotEmpty()
            .WithMessage("Número da demanda é obrigatório")
            .MaximumLength(50)
            .WithMessage("Número da demanda não pode ter mais de 50 caracteres");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Título é obrigatório")
            .MaximumLength(255)
            .WithMessage("Título não pode ter mais de 255 caracteres");

        RuleFor(x => x.AcceptanceCriteria)
            .NotEmpty()
            .WithMessage("Critérios de aceite são obrigatórios");

        RuleFor(x => x.Priority)
            .IsInEnum()
            .WithMessage("Prioridade deve ser um valor válido");

        // Validação da estrutura da história
        When(x => x.UserStory != null, () =>
        {
            RuleFor(x => x.UserStory!.Como)
                .NotEmpty()
                .WithMessage("Campo 'Como' da história é obrigatório");

            RuleFor(x => x.UserStory!.Quero)
                .NotEmpty()
                .WithMessage("Campo 'Quero' da história é obrigatório");

            RuleFor(x => x.UserStory!.Para)
                .NotEmpty()
                .WithMessage("Campo 'Para' da história é obrigatório");
        });

        // Validação dos campos de formulário
        When(x => x.FormFields?.Items != null, () =>
        {
            RuleForEach(x => x.FormFields!.Items)
                .SetValidator(new FormFieldDtoValidator());
        });
    }
}

/// <summary>
/// Validador para campos de formulário
/// </summary>
public class FormFieldDtoValidator : AbstractValidator<FormFieldDto>
{
    public FormFieldDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Nome do campo é obrigatório")
            .MaximumLength(100)
            .WithMessage("Nome do campo não pode ter mais de 100 caracteres");

        RuleFor(x => x.Type)
            .NotEmpty()
            .WithMessage("Tipo do campo é obrigatório")
            .Must(BeValidFieldType)
            .WithMessage("Tipo do campo deve ser válido (text, number, date, datetime, boolean, select, button)");

        // Validação condicional do tamanho
        When(x => x.Type == "text" || x.Type == "number", () =>
        {
            RuleFor(x => x.Size)
                .NotEmpty()
                .WithMessage("Tamanho é obrigatório para campos de texto e número");
        });
    }

    private bool BeValidFieldType(string type)
    {
        var validTypes = new[] { "text", "number", "date", "datetime", "boolean", "select", "button" };
        return validTypes.Contains(type);
    }
}

using IntegrationAzure.Api.Domain.Entities;

namespace IntegrationAzure.Api.Application.DTOs;

/// <summary>
/// DTO para criação de nova história de usuário
/// Baseado no formulário StoryForm.jsx completo
/// </summary>
public class CreateUserStoryDto
{
    public string DemandNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string AcceptanceCriteria { get; set; } = string.Empty;
    public Priority Priority { get; set; } = Priority.Medium;

    // História do usuário (como/quero/para)
    public UserStoryStructureDto? UserStory { get; set; }

    // Seções opcionais
    public ImpactSectionDto? Impact { get; set; }
    public ObjectiveSectionDto? Objective { get; set; }
    public ScreenshotsSectionDto? Screenshots { get; set; }
    public FormFieldsSectionDto? FormFields { get; set; }
    public MessagesSectionDto? Messages { get; set; }
    public BusinessRulesSectionDto? BusinessRules { get; set; }
    public ScenariosSectionDto? Scenarios { get; set; }
    public AttachmentsSectionDto? Attachments { get; set; }
}

/// <summary>
/// Estrutura da história do usuário (como/quero/para)
/// </summary>
public class UserStoryStructureDto
{
    public string Como { get; set; } = string.Empty;
    public string Quero { get; set; } = string.Empty;
    public string Para { get; set; } = string.Empty;
}

/// <summary>
/// Seção de impacto
/// </summary>
public class ImpactSectionDto
{
    public List<ImpactItemDto> Items { get; set; } = new();
}

public class ImpactItemDto
{
    public string Current { get; set; } = string.Empty;
    public string Expected { get; set; } = string.Empty;
}

/// <summary>
/// Seção de objetivos
/// </summary>
public class ObjectiveSectionDto
{
    public List<ObjectiveFieldDto> Fields { get; set; } = new();
}

public class ObjectiveFieldDto
{
    public string Content { get; set; } = string.Empty;
}

/// <summary>
/// Seção de telas ilustrativas
/// </summary>
public class ScreenshotsSectionDto
{
    public List<FileInfoDto> Items { get; set; } = new();
}

/// <summary>
/// Seção de campos de preenchimento
/// </summary>
public class FormFieldsSectionDto
{
    public List<FormFieldDto> Items { get; set; } = new();
}

public class FormFieldDto
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public bool Required { get; set; }
}

/// <summary>
/// Seção de mensagens informativas
/// </summary>
public class MessagesSectionDto
{
    public List<MessageFieldDto> Items { get; set; } = new();
}

public class MessageFieldDto
{
    public string Content { get; set; } = string.Empty;
}

/// <summary>
/// Seção de regras de negócio
/// </summary>
public class BusinessRulesSectionDto
{
    public List<BusinessRuleFieldDto> Items { get; set; } = new();
}

public class BusinessRuleFieldDto
{
    public string Content { get; set; } = string.Empty;
}

/// <summary>
/// Seção de cenários
/// </summary>
public class ScenariosSectionDto
{
    public List<ScenarioItemDto> Items { get; set; } = new();
}

public class ScenarioItemDto
{
    public string Given { get; set; } = string.Empty;
    public string When { get; set; } = string.Empty;
    public string Then { get; set; } = string.Empty;
}

/// <summary>
/// Seção de anexos
/// </summary>
public class AttachmentsSectionDto
{
    public List<FileInfoDto> Items { get; set; } = new();
}

/// <summary>
/// Informações de arquivo
/// </summary>
public class FileInfoDto
{
    public string Name { get; set; } = string.Empty;
    public long Size { get; set; }
    public string Type { get; set; } = string.Empty;
}

/// <summary>
/// DTO para atualização de história de usuário
/// </summary>
public class UpdateUserStoryDto
{
    public string? Title { get; set; }
    public string? AcceptanceCriteria { get; set; }
    public string? Description { get; set; }
    public UserStoryStatus? Status { get; set; }
    public Priority? Priority { get; set; }
}

/// <summary>
/// DTO para retorno de história de usuário
/// </summary>
public class UserStoryDto
{
    public Guid Id { get; set; }
    public string DemandNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string AcceptanceCriteria { get; set; } = string.Empty;
    public string? Description { get; set; }
    public UserStoryStatus Status { get; set; }
    public Priority Priority { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
    public List<AttachmentDto> Attachments { get; set; } = new();
}

/// <summary>
/// DTO para resumo de história de usuário (listagem)
/// </summary>
public class UserStorySummaryDto
{
    public Guid Id { get; set; }
    public string DemandNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public UserStoryStatus Status { get; set; }
    public Priority Priority { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public int AttachmentsCount { get; set; }
}

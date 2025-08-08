using System.Text.Json.Serialization;

namespace IntegrationAzure.Api.Application.DTOs;

// DTOs para Azure DevOps
public class AzureConfigurationDto
{
    public string Token { get; set; } = string.Empty;
    public string Organization { get; set; } = string.Empty;
    public string ApiVersion { get; set; } = "7.0";
}

public class AzureProjectDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Visibility { get; set; } = string.Empty;
}

public class AzureWorkItemDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string AssignedTo { get; set; } = string.Empty;
    public string WorkItemType { get; set; } = string.Empty;
}

/// <summary>
/// DTO para criação de work items no Azure DevOps
/// </summary>
public class CreateAzureWorkItemDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? WorkItemType { get; set; } = "User Story";
    public Dictionary<string, object>? AdditionalFields { get; set; }
    public string? DiscussionComment { get; set; }
}

// Classes para deserialização das respostas do Azure DevOps
public class AzureProjectsResponse
{
    public List<AzureProject> Value { get; set; } = new();
}

public class AzureProject
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Visibility { get; set; } = string.Empty;
}

public class AzureWiqlResponse
{
    public List<AzureWorkItemReference> WorkItems { get; set; } = new();
}

public class AzureWorkItemReference
{
    public int Id { get; set; }
    public string Url { get; set; } = string.Empty;
}

public class AzureWorkItemsResponse
{
    public List<AzureWorkItem> Value { get; set; } = new();
}

public class AzureWorkItem
{
    public int Id { get; set; }
    public AzureWorkItemFields? Fields { get; set; }
}

public class AzureWorkItemFields
{
    [JsonPropertyName("System.Title")]
    public string? SystemTitle { get; set; }

    [JsonPropertyName("System.State")]
    public string? SystemState { get; set; }

    [JsonPropertyName("System.WorkItemType")]
    public string? SystemWorkItemType { get; set; }

    [JsonPropertyName("System.AssignedTo")]
    public AzureAssignedTo? SystemAssignedTo { get; set; }
}

public class AzureAssignedTo
{
    public string DisplayName { get; set; } = string.Empty;
    public string UniqueName { get; set; } = string.Empty;
}

public class AzureAttachmentResponse
{
    public string Id { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}

using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using IntegrationAzure.Api.Application.DTOs;
using IntegrationAzure.Api.Domain.Interfaces;

namespace IntegrationAzure.Api.Application.Services;

public class AzureDevOpsService
{
    private readonly IConfigurationRepository _configurationRepository;
    private readonly HttpClient _httpClient;

    public AzureDevOpsService(IConfigurationRepository configurationRepository, HttpClient httpClient)
    {
        _configurationRepository = configurationRepository;
        _httpClient = httpClient;
    }

    public async Task<List<AzureProjectDto>> GetProjectsAsync()
    {
        try
        {
            var azureConfig = await GetAzureConfigurationAsync();
            if (azureConfig == null)
            {
                throw new InvalidOperationException("Configurações do Azure DevOps não encontradas");
            }

            var url = $"https://dev.azure.com/{azureConfig.Organization}/_apis/projects?api-version={azureConfig.ApiVersion}";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", $"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($":{azureConfig.Token}"))}");
            request.Headers.Add("Accept", "application/json");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Erro ao buscar projetos do Azure DevOps: {response.StatusCode} - {errorContent}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var azureResponse = JsonSerializer.Deserialize<AzureProjectsResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return azureResponse?.Value?.Select(p => new AzureProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                State = p.State,
                Visibility = p.Visibility
            }).ToList() ?? new List<AzureProjectDto>();
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao conectar com Azure DevOps: {ex.Message}", ex);
        }
    }

    public async Task<List<AzureWorkItemDto>> GetWorkItemsAsync(string projectId, string workItemType = "User Story")
    {
        try
        {
            var azureConfig = await GetAzureConfigurationAsync();
            if (azureConfig == null)
            {
                throw new InvalidOperationException("Configurações do Azure DevOps não encontradas");
            }

            // Primeiro, buscar o nome do projeto baseado no GUID
            string projectName = projectId;

            // Se o projectId parece ser um GUID, buscar o nome do projeto
            if (Guid.TryParse(projectId, out _))
            {
                var projects = await GetProjectsAsync();
                var project = projects.FirstOrDefault(p => p.Id == projectId);
                if (project != null)
                {
                    projectName = project.Name;
                }
                else
                {
                    throw new InvalidOperationException($"Projeto com ID '{projectId}' não encontrado");
                }
            }

            // Usar WIQL (Work Item Query Language) para buscar work items
            var wiql = $"SELECT [System.Id], [System.Title], [System.State], [System.AssignedTo], [System.WorkItemType] FROM WorkItems WHERE [System.WorkItemType] = '{workItemType}' AND [System.TeamProject] = '{projectName}'";

            var wiqlQuery = new { query = wiql };
            var wiqlContent = new StringContent(JsonSerializer.Serialize(wiqlQuery), Encoding.UTF8, "application/json");

            // Usar versão estável da API para WIQL
            var url = $"https://dev.azure.com/{azureConfig.Organization}/{projectName}/_apis/wit/wiql?api-version=7.0";

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = wiqlContent
            };
            request.Headers.Add("Authorization", $"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($":{azureConfig.Token}"))}");
            request.Headers.Add("Accept", "application/json");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Erro ao buscar work items: {response.StatusCode} - {errorContent}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var wiqlResponse = JsonSerializer.Deserialize<AzureWiqlResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var workItemIds = wiqlResponse?.WorkItems?.Select(wi => wi.Id).ToList();
            if (workItemIds == null || !workItemIds.Any())
            {
                return new List<AzureWorkItemDto>();
            }

            // Buscar detalhes dos work items
            var detailsUrl = $"https://dev.azure.com/{azureConfig.Organization}/_apis/wit/workitems?ids={string.Join(",", workItemIds)}&fields=System.Id,System.Title,System.State,System.AssignedTo,System.WorkItemType&api-version={azureConfig.ApiVersion}";

            var detailsRequest = new HttpRequestMessage(HttpMethod.Get, detailsUrl);
            detailsRequest.Headers.Add("Authorization", $"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($":{azureConfig.Token}"))}");
            detailsRequest.Headers.Add("Accept", "application/json");

            var detailsResponse = await _httpClient.SendAsync(detailsRequest);

            if (!detailsResponse.IsSuccessStatusCode)
            {
                var errorContent = await detailsResponse.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Erro ao buscar detalhes dos work items: {detailsResponse.StatusCode} - {errorContent}");
            }

            var detailsContent = await detailsResponse.Content.ReadAsStringAsync();
            var workItemsResponse = JsonSerializer.Deserialize<AzureWorkItemsResponse>(detailsContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return workItemsResponse?.Value?.Select(wi => new AzureWorkItemDto
            {
                Id = wi.Id.ToString(),
                Title = wi.Fields?.SystemTitle ?? "Sem título",
                State = wi.Fields?.SystemState ?? "Desconhecido",
                AssignedTo = wi.Fields?.SystemAssignedTo?.DisplayName ?? "Não atribuído",
                WorkItemType = wi.Fields?.SystemWorkItemType ?? workItemType
            }).ToList() ?? new List<AzureWorkItemDto>();
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao buscar work items do Azure DevOps: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Cria um novo work item no Azure DevOps
    /// </summary>
    public async Task<AzureWorkItemDto> CreateWorkItemAsync(string projectId, string workItemType, string title, string description, Dictionary<string, object>? additionalFields = null, string? discussionComment = null)
    {
        try
        {
            var azureConfig = await GetAzureConfigurationAsync();
            if (azureConfig == null)
            {
                throw new InvalidOperationException("Configurações do Azure DevOps não encontradas");
            }

            var url = $"https://dev.azure.com/{azureConfig.Organization}/{projectId}/_apis/wit/workitems/${workItemType}?api-version={azureConfig.ApiVersion}";

            // Preparar o payload do work item (formato PATCH JSON)
            var operations = new List<object>
            {
                new
                {
                    op = "add",
                    path = "/fields/System.Title",
                    value = title
                },
                new
                {
                    op = "add",
                    path = "/fields/System.Description",
                    value = description
                }
            };

            // Adicionar campos adicionais se fornecidos
            if (additionalFields != null)
            {
                foreach (var field in additionalFields)
                {
                    operations.Add(new
                    {
                        op = "add",
                        path = $"/fields/{field.Key}",
                        value = field.Value
                    });
                }
            }

            var jsonContent = JsonSerializer.Serialize(operations);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json-patch+json");

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content
            };
            request.Headers.Add("Authorization", $"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($":{azureConfig.Token}"))}");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Erro ao criar work item: {response.StatusCode} - {errorContent}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var createdWorkItem = JsonSerializer.Deserialize<AzureWorkItem>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // Se há um comentário de discussão, adicionar como comentário no work item
            if (!string.IsNullOrEmpty(discussionComment) && createdWorkItem?.Id > 0)
            {
                try
                {
                    await AddWorkItemCommentAsync(azureConfig, projectId, createdWorkItem.Id, discussionComment);
                }
                catch (Exception commentEx)
                {
                }
            }

            return new AzureWorkItemDto
            {
                Id = createdWorkItem?.Id.ToString() ?? "0",
                Title = createdWorkItem?.Fields?.SystemTitle ?? title,
                State = createdWorkItem?.Fields?.SystemState ?? "New",
                AssignedTo = createdWorkItem?.Fields?.SystemAssignedTo?.DisplayName ?? "Não atribuído",
                WorkItemType = createdWorkItem?.Fields?.SystemWorkItemType ?? workItemType
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao criar work item no Azure DevOps: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Adiciona um comentário a um work item existente
    /// </summary>
    private async Task AddWorkItemCommentAsync(AzureConfigurationDto azureConfig, string projectId, int workItemId, string comment)
    {
        var url = $"https://dev.azure.com/{azureConfig.Organization}/{projectId}/_apis/wit/workItems/{workItemId}/comments?api-version={azureConfig.ApiVersion}";

        var commentPayload = new
        {
            text = comment
        };

        var jsonContent = JsonSerializer.Serialize(commentPayload);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = content
        };
        request.Headers.Add("Authorization", $"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($":{azureConfig.Token}"))}");

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Erro ao adicionar comentário ao work item: {response.StatusCode} - {errorContent}");
        }
    }

    private async Task<AzureConfigurationDto?> GetAzureConfigurationAsync()
    {
        var configurations = await _configurationRepository.GetByKeysAsync(new[] { "Azure_Token", "Organizacao", "Versao_API" });

        var token = configurations.FirstOrDefault(c => c.Key == "Azure_Token")?.Value;
        var organization = configurations.FirstOrDefault(c => c.Key == "Organizacao")?.Value;
        var apiVersion = configurations.FirstOrDefault(c => c.Key == "Versao_API")?.Value ?? "7.0";

        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(organization))
        {
            return null;
        }

        return new AzureConfigurationDto
        {
            Token = token,
            Organization = organization,
            ApiVersion = apiVersion
        };
    }
}

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

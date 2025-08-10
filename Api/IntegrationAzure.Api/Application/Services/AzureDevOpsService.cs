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
    /// Busca os valores permitidos (allowedValues) do campo Activity. Esse campo normalmente EXISTE no tipo 'Task'.
    /// Em muitos processos (Agile/Scrum) ele NÃO está presente em 'Bug', por isso tentamos múltiplos tipos.
    /// Fallback: retorna lista padrão caso nada seja encontrado via API.
    /// </summary>
    public async Task<List<string>> GetActivitiesAsync(string projectId, string workItemType = "Task", string fieldRefName = "Microsoft.VSTS.Common.Activity")
    {
        var azureConfig = await GetAzureConfigurationAsync() ?? throw new InvalidOperationException("Configurações do Azure DevOps não encontradas");

        // Converter projectId GUID para nome, se necessário
        string projectName = projectId;
        if (Guid.TryParse(projectId, out _))
        {
            var projects = await GetProjectsAsync();
            var project = projects.FirstOrDefault(p => p.Id == projectId);
            if (project != null) projectName = project.Name; else throw new InvalidOperationException($"Projeto com ID '{projectId}' não encontrado");
        }

        // Monta lista de tipos a tentar (prioriza o solicitado, depois Task e Bug sem duplicar)
        var typesToTry = new List<string>();
        void AddType(string t)
        {
            if (!typesToTry.Contains(t, StringComparer.OrdinalIgnoreCase)) typesToTry.Add(t);
        }
        AddType(workItemType);
        AddType("Task");
        AddType("Bug");

        foreach (var type in typesToTry)
        {
            var url = $"https://dev.azure.com/{azureConfig.Organization}/{projectName}/_apis/wit/workitemtypes/{Uri.EscapeDataString(type)}/fields/{Uri.EscapeDataString(fieldRefName)}?api-version={azureConfig.ApiVersion}";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", $"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($":{azureConfig.Token}"))}");
            request.Headers.Add("Accept", "application/json");

            try
            {
                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    // Se 404 ou campo não aplicável, tenta próximo tipo
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        continue;
                    var err = await response.Content.ReadAsStringAsync();
                    // Erros de campo não existente para aquele tipo: continuar
                    if (err.Contains("TF401326", StringComparison.OrdinalIgnoreCase) ||
                        err.Contains("does not exist", StringComparison.OrdinalIgnoreCase))
                        continue;
                    // Outros erros: lançar
                    throw new HttpRequestException($"Erro ao buscar Activities no tipo '{type}': {response.StatusCode} - {err}");
                }

                var content = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(content);
                var root = doc.RootElement;
                if (root.TryGetProperty("allowedValues", out var allowedValues) && allowedValues.ValueKind == JsonValueKind.Array && allowedValues.GetArrayLength() > 0)
                {
                    var list = new List<string>();
                    foreach (var item in allowedValues.EnumerateArray())
                    {
                        if (item.ValueKind == JsonValueKind.String)
                            list.Add(item.GetString()!);
                        else if (item.TryGetProperty("value", out var valueEl) && valueEl.ValueKind == JsonValueKind.String)
                            list.Add(valueEl.GetString()!);
                    }
                    if (list.Count > 0)
                        return list.Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(x => x).ToList();
                }
            }
            catch (Exception ex)
            {
                // Continua tentando outros tipos em caso de erro específico; se último tipo falhar, lança.
                if (type == typesToTry.Last())
                    throw new Exception($"Falha ao obter Activities: {ex.Message}", ex);
            }
        }

        // Fallback padrão caso nada encontrado via API
        var fallback = new List<string>
        {
            "Development",
            "Design",
            "Testing",
            "Documentation",
            "Requirements",
            "Deployment",
            "Research"
        };
        return fallback;
    }

    /// <summary>
    /// Cria um novo work item no Azure DevOps
    /// </summary>
    public async Task<AzureWorkItemDto> CreateWorkItemAsync(string projectId, string workItemType, string title, string description, Dictionary<string, object>? additionalFields = null, string? discussionComment = null, List<(byte[] content, string fileName)>? attachments = null)
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
                catch (Exception)
                {
                    // Ignora erro ao adicionar comentário
                }
            }

            // Se há anexos, fazer upload e anexar ao work item
            if (attachments?.Any() == true && createdWorkItem?.Id > 0)
            {
                foreach (var attachment in attachments)
                {
                    try
                    {
                        // Fazer upload do anexo
                        var attachmentId = await UploadAttachmentAsync(azureConfig, projectId, attachment.content, attachment.fileName);

                        if (!string.IsNullOrEmpty(attachmentId))
                        {
                            // Anexar ao work item
                            await AttachFileToWorkItemAsync(azureConfig, projectId, createdWorkItem.Id, attachmentId, attachment.fileName);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log do erro, mas não quebra o processo
                        Console.WriteLine($"Erro ao anexar arquivo '{attachment.fileName}': {ex.Message}");
                    }
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
    /// Cria um novo work item no Azure DevOps com relacionamento a outro work item
    /// </summary>
    public async Task<AzureWorkItemDto> CreateWorkItemWithRelationAsync(
        string projectId,
        string workItemType,
        string title,
        string description,
        int relatedWorkItemId,
        string relationType = "System.LinkTypes.Hierarchy-Reverse", // Child por padrão
        Dictionary<string, object>? additionalFields = null,
        string? discussionComment = null,
        List<(byte[] content, string fileName)>? attachments = null)
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

            // Adicionar relacionamento com o work item pai
            operations.Add(new
            {
                op = "add",
                path = "/relations/-",
                value = new
                {
                    rel = relationType,
                    url = $"https://dev.azure.com/{azureConfig.Organization}/_apis/wit/workItems/{relatedWorkItemId}"
                }
            });

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
                throw new HttpRequestException($"Erro ao criar work item relacionado no Azure DevOps: {response.StatusCode} - {errorContent}. URL: {url}");
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
                catch (Exception)
                {
                    // Log do erro, mas não quebra o processo - ex não usado intencionalmente
                }
            }

            // Se há anexos, fazer upload e anexar ao work item
            if (attachments?.Any() == true && createdWorkItem?.Id > 0)
            {
                foreach (var attachment in attachments)
                {
                    try
                    {
                        // Fazer upload do anexo
                        var attachmentId = await UploadAttachmentAsync(azureConfig, projectId, attachment.content, attachment.fileName);

                        if (!string.IsNullOrEmpty(attachmentId))
                        {
                            // Anexar ao work item
                            await AttachFileToWorkItemAsync(azureConfig, projectId, createdWorkItem.Id, attachmentId, attachment.fileName);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log do erro, mas não quebra o processo
                        Console.WriteLine($"Erro ao anexar arquivo '{attachment.fileName}': {ex.Message}");
                    }
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
            throw new InvalidOperationException($"Erro ao criar work item relacionado: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Adiciona um comentário a um work item existente
    /// </summary>
    /// <summary>
    /// Adiciona discussão (discussion) a um work item existente
    /// </summary>
    private async Task AddWorkItemCommentAsync(AzureConfigurationDto azureConfig, string projectId, int workItemId, string comment)
    {
        var url = $"https://dev.azure.com/{azureConfig.Organization}/{projectId}/_apis/wit/workitems/{workItemId}?api-version={azureConfig.ApiVersion}";

        // Usar PATCH para adicionar à Discussion do work item
        var operations = new List<object>
        {
            new
            {
                op = "add",
                path = "/fields/System.History",
                value = comment
            }
        };

        var jsonContent = JsonSerializer.Serialize(operations);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json-patch+json");

        var request = new HttpRequestMessage(HttpMethod.Patch, url)
        {
            Content = content
        };
        request.Headers.Add("Authorization", $"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($":{azureConfig.Token}"))}");
        request.Headers.Add("Accept", "application/json");

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Erro ao adicionar discussão ao work item: {response.StatusCode} - {errorContent}. URL: {url}");
        }
    }

    /// <summary>
    /// Faz upload de um anexo para o Azure DevOps e retorna o ID do anexo
    /// </summary>
    private async Task<string?> UploadAttachmentAsync(AzureConfigurationDto azureConfig, string projectId, byte[] fileContent, string fileName)
    {
        var url = $"https://dev.azure.com/{azureConfig.Organization}/{projectId}/_apis/wit/attachments?fileName={Uri.EscapeDataString(fileName)}&api-version={azureConfig.ApiVersion}";

        var content = new ByteArrayContent(fileContent);
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = content
        };
        request.Headers.Add("Authorization", $"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($":{azureConfig.Token}"))}");

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Erro ao fazer upload do anexo '{fileName}': {response.StatusCode} - {errorContent}. URL: {url}");
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        var attachmentResponse = JsonSerializer.Deserialize<AzureAttachmentResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return attachmentResponse?.Id;
    }

    /// <summary>
    /// Anexa um arquivo ao work item usando o ID do anexo obtido pelo upload
    /// </summary>
    private async Task AttachFileToWorkItemAsync(AzureConfigurationDto azureConfig, string projectId, int workItemId, string attachmentId, string fileName)
    {
        var url = $"https://dev.azure.com/{azureConfig.Organization}/{projectId}/_apis/wit/workitems/{workItemId}?api-version={azureConfig.ApiVersion}";

        var operations = new List<object>
        {
            new
            {
                op = "add",
                path = "/relations/-",
                value = new
                {
                    rel = "AttachedFile",
                    url = $"https://dev.azure.com/{azureConfig.Organization}/_apis/wit/attachments/{attachmentId}",
                    attributes = new
                    {
                        name = fileName,
                        comment = $"Anexo: {fileName}"
                    }
                }
            }
        };

        var jsonContent = JsonSerializer.Serialize(operations);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json-patch+json");

        var request = new HttpRequestMessage(HttpMethod.Patch, url)
        {
            Content = content
        };
        request.Headers.Add("Authorization", $"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($":{azureConfig.Token}"))}");

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Erro ao anexar arquivo '{fileName}' ao work item: {response.StatusCode} - {errorContent}. URL: {url}");
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

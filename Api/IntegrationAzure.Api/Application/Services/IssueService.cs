using IntegrationAzure.Api.Application.DTOs;
using IntegrationAzure.Api.Domain.Entities;
using IntegrationAzure.Api.Domain.Interfaces;

namespace IntegrationAzure.Api.Application.Services;

/// <summary>
/// Serviço para operações relacionadas a issues
/// Implementa regras de negócio e orchestração
/// </summary>
public class IssueService
{
    private readonly IIssueRepository _issueRepository;
    private readonly IUserStoryRepository _userStoryRepository;
    private readonly HtmlGeneratorService _htmlGeneratorService;
    private readonly AzureDevOpsService _azureDevOpsService;

    public IssueService(
        IIssueRepository issueRepository,
        IUserStoryRepository userStoryRepository,
        HtmlGeneratorService htmlGeneratorService,
        AzureDevOpsService azureDevOpsService)
    {
        _issueRepository = issueRepository ?? throw new ArgumentNullException(nameof(issueRepository));
        _userStoryRepository = userStoryRepository ?? throw new ArgumentNullException(nameof(userStoryRepository));
        _htmlGeneratorService = htmlGeneratorService ?? throw new ArgumentNullException(nameof(htmlGeneratorService));
        _azureDevOpsService = azureDevOpsService ?? throw new ArgumentNullException(nameof(azureDevOpsService));
    }

    /// <summary>
    /// Cria uma nova issue
    /// </summary>
    public async Task<ApiResponseDto<IssueDto>> CreateAsync(CreateIssueDto dto, string currentUser)
    {
        // Converter anexos do DTO para formato binário se existirem
        List<(byte[] content, string fileName)>? attachments = null;

        Console.WriteLine($"[DEBUG] Processando issue - Anexos recebidos: {dto.Attachments?.Count ?? 0}");

        if (dto.Attachments?.Any() == true)
        {
            attachments = new List<(byte[] content, string fileName)>();

            foreach (var attachment in dto.Attachments)
            {
                Console.WriteLine($"[DEBUG] Processando anexo: {attachment.FileName} - Content null? {string.IsNullOrEmpty(attachment.Content)}");

                if (!string.IsNullOrEmpty(attachment.Content))
                {
                    try
                    {
                        // Converter de base64 para bytes
                        var bytes = Convert.FromBase64String(attachment.Content);
                        attachments.Add((bytes, attachment.FileName));
                        Console.WriteLine($"[DEBUG] Anexo convertido com sucesso: {attachment.FileName} - {bytes.Length} bytes");
                    }
                    catch (FormatException ex)
                    {
                        Console.WriteLine($"[DEBUG] Erro ao converter base64 para {attachment.FileName}: {ex.Message}");
                        // Se não conseguir converter base64, pular este anexo
                        continue;
                    }
                }
                else
                {
                    Console.WriteLine($"[DEBUG] Anexo {attachment.FileName} sem conteúdo, pulando...");
                }
            }
        }

        Console.WriteLine($"[DEBUG] Total de anexos processados: {attachments?.Count ?? 0}");
        return await CreateInternalAsync(dto, currentUser, attachments);
    }

    /// <summary>
    /// Método interno para criação de issue
    /// </summary>
    private async Task<ApiResponseDto<IssueDto>> CreateInternalAsync(CreateIssueDto dto, string currentUser, List<(byte[] content, string fileName)>? attachments)
    {
        try
        {
            // Gerar a descrição em HTML usando os dados estruturados
            var htmlDescription = _htmlGeneratorService.GenerateIssueDescription(dto, dto.Observations);

            var issue = new Issue
            {
                IssueNumber = dto.IssueNumber ?? $"ISS-{DateTime.UtcNow:yyyyMMdd}-{DateTime.UtcNow.Ticks.ToString().Substring(10)}",
                Title = dto.Title,
                Description = htmlDescription, // Usar a descrição gerada em HTML
                Type = dto.Type,
                Priority = dto.Priority,
                Activity = dto.Activity, // Campo Activity para Azure DevOps
                Environment = dto.Environment,
                UserStoryId = dto.UserStoryId,
                CreatedBy = currentUser,
                Status = IssueStatus.Open
            };

            await _issueRepository.AddAsync(issue);
            await _issueRepository.SaveChangesAsync();

            var createdIssue = await _issueRepository.GetWithAttachmentsAsync(issue.Id);

            // Automaticamente criar no Azure DevOps após salvar localmente
            try
            {
                var azureProjects = await _azureDevOpsService.GetProjectsAsync();
                if (azureProjects?.Any() == true)
                {
                    // Usar o primeiro projeto disponível ou o projeto padrão
                    var targetProject = azureProjects.First();

                    // Preparar campos adicionais baseados no tipo e prioridade
                    var additionalFields = new Dictionary<string, object>
                    {
                        ["Microsoft.VSTS.Common.Severity"] = GetAzureSeverityFromPriority(issue.Priority),
                        ["Microsoft.VSTS.Common.Priority"] = GetAzurePriorityValue(issue.Priority),
                        ["System.AreaPath"] = targetProject.Name,
                        ["System.IterationPath"] = targetProject.Name,
                        ["Microsoft.VSTS.Common.Activity"] = issue.Activity ?? "Desenvolvimento" // Adicionar Activity
                    };

                    // Determinar o tipo de work item baseado no tipo da issue
                    var workItemType = GetAzureWorkItemType(issue.Type);

                    // Se a issue está associada a uma User Story, buscar no Azure DevOps e criar relacionamento
                    if (issue.UserStoryId.HasValue)
                    {
                        try
                        {
                            // Buscar work items User Stories para encontrar a correspondente
                            var workItems = await _azureDevOpsService.GetWorkItemsAsync(targetProject.Name, "User Story");
                            var relatedUserStory = workItems?.FirstOrDefault(wi =>
                                wi.Title.Contains($"US-{issue.UserStoryId}") ||
                                wi.Id == issue.UserStoryId.Value.ToString());

                            if (relatedUserStory != null && int.TryParse(relatedUserStory.Id, out int userStoryWorkItemId))
                            {
                                // Criar Work Item relacionado à User Story
                                var azureWorkItem = await _azureDevOpsService.CreateWorkItemWithRelationAsync(
                                    targetProject.Name,
                                    workItemType,
                                    $"[{issue.IssueNumber}] {issue.Title}",
                                    $"Issue registrada em {issue.Environment ?? "Não especificado"}",
                                    userStoryWorkItemId,
                                    "System.LinkTypes.Hierarchy-Reverse", // Work Item como child da User Story
                                    additionalFields,
                                    htmlDescription, // HTML vai para a discussão
                                    attachments // Anexos enviados
                                );
                            }
                            else
                            {
                                // Se não encontrar User Story relacionada, criar Work Item sem relacionamento
                                var azureWorkItem = await _azureDevOpsService.CreateWorkItemAsync(
                                    targetProject.Name,
                                    workItemType,
                                    $"[{issue.IssueNumber}] {issue.Title}",
                                    $"Issue registrada em {issue.Environment ?? "Não especificado"}",
                                    additionalFields,
                                    htmlDescription,
                                    attachments // Anexos enviados
                                );
                            }
                        }
                        catch (Exception ex)
                        {
                            // Log do erro, mas não quebra o processo
                            Console.WriteLine($"Erro ao criar Work Item relacionado no Azure DevOps: {ex.Message}");
                        }
                    }
                    else
                    {
                        // Criar Work Item sem relacionamento se não há UserStoryId
                        var azureWorkItem = await _azureDevOpsService.CreateWorkItemAsync(
                            targetProject.Name,
                            workItemType,
                            $"[{issue.IssueNumber}] {issue.Title}",
                            $"Issue registrada em {issue.Environment ?? "Não especificado"}",
                            additionalFields,
                            htmlDescription,
                            attachments // Anexos enviados
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                // Log do erro, mas não quebra o processo principal
                Console.WriteLine($"Erro ao integrar issue com Azure DevOps: {ex.Message}");
            }

            return new ApiResponseDto<IssueDto>
            {
                Success = true,
                Message = "Issue criada com sucesso",
                Data = MapToDto(createdIssue!)
            };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<IssueDto>
            {
                Success = false,
                Message = "Erro interno do servidor",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    /// <summary>
    /// Obtém uma issue por ID
    /// </summary>
    public async Task<ApiResponseDto<IssueDto>> GetByIdAsync(Guid id)
    {
        try
        {
            var issue = await _issueRepository.GetWithAttachmentsAsync(id);

            if (issue == null)
            {
                return new ApiResponseDto<IssueDto>
                {
                    Success = false,
                    Message = "Issue não encontrada"
                };
            }

            return new ApiResponseDto<IssueDto>
            {
                Success = true,
                Message = "Issue encontrada",
                Data = MapToDto(issue)
            };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<IssueDto>
            {
                Success = false,
                Message = "Erro interno do servidor",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    /// <summary>
    /// Obtém todas as issues
    /// </summary>
    public async Task<ApiResponseDto<List<IssueSummaryDto>>> GetAllAsync()
    {
        try
        {
            var issues = await _issueRepository.GetAllAsync();

            var summaryDtos = issues.Select(i => new IssueSummaryDto
            {
                Id = i.Id,
                IssueNumber = i.IssueNumber,
                Title = i.Title,
                Type = i.Type,
                Priority = i.Priority,
                Status = i.Status,
                Activity = i.Activity, // Campo Activity para Azure DevOps (substitui OccurrenceType)
                Environment = i.Environment,
                UserStoryId = i.UserStoryId, // Adicionando UserStoryId
                CreatedAt = i.CreatedAt,
                CreatedBy = i.CreatedBy
            }).ToList();

            return new ApiResponseDto<List<IssueSummaryDto>>
            {
                Success = true,
                Message = "Issues encontradas",
                Data = summaryDtos
            };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<List<IssueSummaryDto>>
            {
                Success = false,
                Message = "Erro interno do servidor",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    /// <summary>
    /// Mapeia entidade para DTO
    /// </summary>
    private IssueDto MapToDto(Issue issue)
    {
        return new IssueDto
        {
            Id = issue.Id,
            IssueNumber = issue.IssueNumber,
            Title = issue.Title,
            Description = issue.Description,
            Type = issue.Type,
            Priority = issue.Priority,
            Status = issue.Status,
            Activity = issue.Activity, // Campo Activity para Azure DevOps (substitui OccurrenceType)
            Environment = issue.Environment,
            CreatedAt = issue.CreatedAt,
            CreatedBy = issue.CreatedBy,
            UserStoryId = issue.UserStoryId,
            Attachments = issue.Attachments.Select(a => new AttachmentDto
            {
                Id = a.Id,
                FileName = a.FileName,
                OriginalFileName = a.OriginalFileName,
                ContentType = a.ContentType,
                Size = a.Size,
                Description = a.Description,
                CreatedAt = a.CreatedAt,
                CreatedBy = a.CreatedBy
            }).ToList()
        };
    }

    /// <summary>
    /// Mapeia tipo de issue interna para tipo de work item do Azure DevOps
    /// </summary>
    private static string GetAzureWorkItemType(IssueType type)
    {
        return type switch
        {
            IssueType.Bug => "Bug",
            IssueType.Feature => "Feature",
            IssueType.Improvement => "Product Backlog Item",
            IssueType.Task => "Task",
            _ => "Bug"
        };
    }

    /// <summary>
    /// Mapeia prioridade da issue para severidade do Azure DevOps
    /// </summary>
    private static string GetAzureSeverityFromPriority(Priority priority)
    {
        return priority switch
        {
            Priority.Critical => "1 - Critical",
            Priority.High => "2 - High",
            Priority.Medium => "3 - Medium",
            Priority.Low => "4 - Low",
            _ => "3 - Medium"
        };
    }

    /// <summary>
    /// Mapeia prioridade da issue para prioridade do Azure DevOps
    /// </summary>
    private static int GetAzurePriorityValue(Priority priority)
    {
        return priority switch
        {
            Priority.Critical => 1,
            Priority.High => 2,
            Priority.Medium => 3,
            Priority.Low => 4,
            _ => 3
        };
    }
}

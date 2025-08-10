using IntegrationAzure.Api.Application.DTOs;
using IntegrationAzure.Api.Domain.Entities;
using IntegrationAzure.Api.Domain.Interfaces;

namespace IntegrationAzure.Api.Application.Services;

/// <summary>
/// Serviço para operações relacionadas a falhas
/// Implementa regras de negócio e orchestração
/// </summary>
public class FailureService
{
    private readonly IFailureRepository _failureRepository;
    private readonly IUserStoryRepository _userStoryRepository;
    private readonly HtmlGeneratorService _htmlGenerator;
    private readonly AzureDevOpsService _azureDevOpsService;

    public FailureService(
        IFailureRepository failureRepository,
        IUserStoryRepository userStoryRepository,
        HtmlGeneratorService htmlGenerator,
        AzureDevOpsService azureDevOpsService)
    {
        _failureRepository = failureRepository ?? throw new ArgumentNullException(nameof(failureRepository));
        _userStoryRepository = userStoryRepository ?? throw new ArgumentNullException(nameof(userStoryRepository));
        _htmlGenerator = htmlGenerator ?? throw new ArgumentNullException(nameof(htmlGenerator));
        _azureDevOpsService = azureDevOpsService ?? throw new ArgumentNullException(nameof(azureDevOpsService));
    }

    /// <summary>
    /// Cria uma nova falha
    /// </summary>
    public async Task<ApiResponseDto<FailureDto>> CreateAsync(CreateFailureDto dto, string currentUser)
    {
        try
        {
            // Gerar o número da falha automaticamente se não fornecido
            var failureNumber = !string.IsNullOrEmpty(dto.FailureNumber)
                ? dto.FailureNumber
                : $"FLH-{DateTime.UtcNow:yyyyMMdd}-{DateTime.UtcNow.Ticks.ToString().Substring(10)}";

            // Gerar a descrição em HTML a partir dos dados estruturados
            var htmlDescription = _htmlGenerator.GenerateFailureDescription(dto, dto.Scenarios, dto.Observations);

            var failure = new Failure
            {
                FailureNumber = failureNumber,
                Title = dto.Title ?? string.Empty,
                Description = htmlDescription, // Descrição gerada em HTML
                Severity = dto.Severity ?? FailureSeverity.Medium,
                Activity = dto.Activity,
                OccurredAt = dto.OccurredAt ?? DateTime.UtcNow,
                Environment = dto.Environment,
                UserStoryId = dto.UserStoryId,
                CreatedBy = currentUser,
                Status = FailureStatus.Reported
            };

            await _failureRepository.AddAsync(failure);
            await _failureRepository.SaveChangesAsync();

            var createdFailure = await _failureRepository.GetWithAttachmentsAsync(failure.Id);

            // Automaticamente criar no Azure DevOps após salvar localmente
            try
            {
                var azureProjects = await _azureDevOpsService.GetProjectsAsync();
                if (azureProjects?.Any() == true)
                {
                    // Usar o primeiro projeto disponível ou o projeto padrão
                    var targetProject = azureProjects.First();

                    // Preparar campos adicionais baseados na severidade
                    var additionalFields = new Dictionary<string, object>
                    {
                        ["Microsoft.VSTS.Common.Severity"] = GetAzureSeverity(failure.Severity),
                        ["Microsoft.VSTS.Common.Priority"] = GetAzurePriorityFromSeverity(failure.Severity),
                        ["System.AreaPath"] = targetProject.Name,
                        ["System.IterationPath"] = targetProject.Name,
                        ["Microsoft.VSTS.TCM.ReproSteps"] = htmlDescription // Passos para reproduzir
                    };

                    // Se a falha está associada a uma User Story, buscar no Azure DevOps e criar relacionamento
                    if (failure.UserStoryId.HasValue)
                    {
                        try
                        {
                            // Buscar work items User Stories para encontrar a correspondente
                            var workItems = await _azureDevOpsService.GetWorkItemsAsync(targetProject.Name, "User Story");
                            var relatedUserStory = workItems?.FirstOrDefault(wi =>
                                wi.Title.Contains($"US-{failure.UserStoryId}") ||
                                wi.Id == failure.UserStoryId.Value.ToString());

                            if (relatedUserStory != null && int.TryParse(relatedUserStory.Id, out int userStoryWorkItemId))
                            {
                                // Criar Bug relacionado à User Story
                                var azureBug = await _azureDevOpsService.CreateWorkItemWithRelationAsync(
                                    targetProject.Name,
                                    "Bug",
                                    $"[{failure.FailureNumber}] {failure.Title}",
                                    $"Falha reportada em {failure.Environment ?? "Não especificado"}",
                                    userStoryWorkItemId,
                                    "System.LinkTypes.Hierarchy-Reverse", // Bug como child da User Story
                                    additionalFields,
                                    htmlDescription, // HTML vai para a discussão
                                    null // Anexos seriam implementados posteriormente
                                );

                                // Você pode salvar o azureBug.Id na entidade se necessário
                            }
                            else
                            {
                                // Se não encontrar User Story relacionada, criar Bug sem relacionamento
                                var azureBug = await _azureDevOpsService.CreateWorkItemAsync(
                                    targetProject.Name,
                                    "Bug",
                                    $"[{failure.FailureNumber}] {failure.Title}",
                                    $"Falha reportada em {failure.Environment ?? "Não especificado"}",
                                    additionalFields,
                                    htmlDescription,
                                    null
                                );
                            }
                        }
                        catch (Exception ex)
                        {
                            // Log do erro, mas não quebra o processo
                            Console.WriteLine($"Erro ao criar Bug relacionado no Azure DevOps: {ex.Message}");
                        }
                    }
                    else
                    {
                        // Criar Bug sem relacionamento se não há UserStoryId
                        var azureBug = await _azureDevOpsService.CreateWorkItemAsync(
                            targetProject.Name,
                            "Bug",
                            $"[{failure.FailureNumber}] {failure.Title}",
                            $"Falha reportada em {failure.Environment ?? "Não especificado"}",
                            additionalFields,
                            htmlDescription,
                            null
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                // Log do erro, mas não quebra o processo principal
                Console.WriteLine($"Erro ao integrar falha com Azure DevOps: {ex.Message}");
            }

            return new ApiResponseDto<FailureDto>
            {
                Success = true,
                Message = "Falha criada com sucesso",
                Data = MapToDto(createdFailure!)
            };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<FailureDto>
            {
                Success = false,
                Message = "Erro interno do servidor",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    /// <summary>
    /// Obtém uma falha por ID
    /// </summary>
    public async Task<ApiResponseDto<FailureDto>> GetByIdAsync(Guid id)
    {
        try
        {
            var failure = await _failureRepository.GetWithAttachmentsAsync(id);

            if (failure == null)
            {
                return new ApiResponseDto<FailureDto>
                {
                    Success = false,
                    Message = "Falha não encontrada"
                };
            }

            return new ApiResponseDto<FailureDto>
            {
                Success = true,
                Message = "Falha encontrada",
                Data = MapToDto(failure)
            };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<FailureDto>
            {
                Success = false,
                Message = "Erro interno do servidor",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    /// <summary>
    /// Obtém todas as falhas
    /// </summary>
    public async Task<ApiResponseDto<List<FailureSummaryDto>>> GetAllAsync()
    {
        try
        {
            var failures = await _failureRepository.GetAllAsync();

            var summaryDtos = failures.Select(f => new FailureSummaryDto
            {
                Id = f.Id,
                FailureNumber = f.FailureNumber,
                Title = f.Title,
                Severity = f.Severity,
                Status = f.Status,
                Activity = f.Activity,
                OccurredAt = f.OccurredAt,
                CreatedAt = f.CreatedAt,
                CreatedBy = f.CreatedBy,
                UserStoryTitle = null, // Não há relacionamento direto
                AttachmentsCount = f.Attachments.Count
            }).ToList();

            return new ApiResponseDto<List<FailureSummaryDto>>
            {
                Success = true,
                Message = "Falhas encontradas",
                Data = summaryDtos
            };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<List<FailureSummaryDto>>
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
    private FailureDto MapToDto(Failure failure)
    {
        return new FailureDto
        {
            Id = failure.Id,
            FailureNumber = failure.FailureNumber,
            Title = failure.Title,
            Description = failure.Description,
            Severity = failure.Severity,
            Status = failure.Status,
            Activity = failure.Activity,
            OccurredAt = failure.OccurredAt,
            Environment = failure.Environment,
            CreatedAt = failure.CreatedAt,
            CreatedBy = failure.CreatedBy,
            UserStoryId = failure.UserStoryId,
            UserStory = null, // Não há relacionamento direto
            Attachments = failure.Attachments.Select(a => new AttachmentDto
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
    /// Mapeia severidade interna para severidade do Azure DevOps
    /// </summary>
    private static string GetAzureSeverity(FailureSeverity severity)
    {
        return severity switch
        {
            FailureSeverity.Critical => "1 - Critical",
            FailureSeverity.High => "2 - High",
            FailureSeverity.Medium => "3 - Medium",
            FailureSeverity.Low => "4 - Low",
            _ => "3 - Medium"
        };
    }

    /// <summary>
    /// Mapeia severidade da falha para prioridade do Azure DevOps
    /// </summary>
    private static int GetAzurePriorityFromSeverity(FailureSeverity severity)
    {
        return severity switch
        {
            FailureSeverity.Critical => 1,
            FailureSeverity.High => 2,
            FailureSeverity.Medium => 3,
            FailureSeverity.Low => 4,
            _ => 3
        };
    }
}

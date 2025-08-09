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
        try
        {
            if (dto.Scenarios != null)
            {
                for (int i = 0; i < dto.Scenarios.Count; i++)
                {
                    var scenario = dto.Scenarios[i];
                }
            }

            // Gerar a descrição em HTML usando os dados estruturados
            var htmlDescription = _htmlGeneratorService.GenerateIssueDescription(dto);

            var issue = new Issue
            {
                IssueNumber = dto.IssueNumber,
                Title = dto.Title,
                Description = htmlDescription, // Usar a descrição gerada em HTML
                Type = dto.Type,
                Priority = dto.Priority,
                Activity = dto.Activity, // Campo Activity para Azure DevOps (substitui OccurrenceType)
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
                    var projectName = azureProjects.First().Name;
                }
            }
            catch (Exception ex)
            {
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
}

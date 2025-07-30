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

    public IssueService(
        IIssueRepository issueRepository,
        IUserStoryRepository userStoryRepository)
    {
        _issueRepository = issueRepository ?? throw new ArgumentNullException(nameof(issueRepository));
        _userStoryRepository = userStoryRepository ?? throw new ArgumentNullException(nameof(userStoryRepository));
    }

    /// <summary>
    /// Cria uma nova issue
    /// </summary>
    public async Task<ApiResponseDto<IssueDto>> CreateAsync(CreateIssueDto dto, string currentUser)
    {
        try
        {
            // Verificar se já existe uma issue com o mesmo número
            var existingIssue = await _issueRepository.FirstOrDefaultAsync(i => i.IssueNumber == dto.IssueNumber);
            if (existingIssue != null)
            {
                return new ApiResponseDto<IssueDto>
                {
                    Success = false,
                    Message = "Já existe uma issue com este número",
                    Errors = new List<string> { "Número de issue duplicado" }
                };
            }

            // Verificar se a história de usuário existe (se fornecida)
            if (dto.UserStoryId.HasValue)
            {
                var userStory = await _userStoryRepository.GetByIdAsync(dto.UserStoryId.Value);
                if (userStory == null)
                {
                    return new ApiResponseDto<IssueDto>
                    {
                        Success = false,
                        Message = "História de usuário não encontrada"
                    };
                }
            }

            var issue = new Issue
            {
                IssueNumber = dto.IssueNumber,
                Title = dto.Title,
                Description = dto.Description,
                Type = dto.Type,
                Priority = dto.Priority,
                OccurrenceType = dto.OccurrenceType,
                Environment = dto.Environment,
                UserStoryId = dto.UserStoryId,
                CreatedBy = currentUser,
                Status = IssueStatus.Open
            };

            await _issueRepository.AddAsync(issue);
            await _issueRepository.SaveChangesAsync();

            var createdIssue = await _issueRepository.GetWithAttachmentsAsync(issue.Id);

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
                OccurrenceType = i.OccurrenceType,
                CreatedAt = i.CreatedAt,
                CreatedBy = i.CreatedBy,
                UserStoryTitle = null, // Não há relacionamento direto
                AttachmentsCount = i.Attachments.Count
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
            OccurrenceType = issue.OccurrenceType,
            Environment = issue.Environment,
            CreatedAt = issue.CreatedAt,
            CreatedBy = issue.CreatedBy,
            UserStoryId = issue.UserStoryId,
            UserStory = null, // Não há relacionamento direto
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

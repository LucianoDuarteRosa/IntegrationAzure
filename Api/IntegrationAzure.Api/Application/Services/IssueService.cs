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
                AssignedTo = dto.AssignedTo,
                Reporter = dto.Reporter,
                Environment = dto.Environment,
                StepsToReproduce = dto.StepsToReproduce,
                ExpectedResult = dto.ExpectedResult,
                ActualResult = dto.ActualResult,
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
                Data = await MapToDtoAsync(createdIssue!)
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
                Data = await MapToDtoAsync(issue)
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
                AssignedTo = i.AssignedTo,
                CreatedAt = i.CreatedAt,
                CreatedBy = i.CreatedBy,
                UserStoryTitle = i.UserStory?.Title,
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
    /// Atualiza uma issue
    /// </summary>
    public async Task<ApiResponseDto<IssueDto>> UpdateAsync(Guid id, UpdateIssueDto dto, string currentUser)
    {
        try
        {
            var issue = await _issueRepository.GetByIdAsync(id);

            if (issue == null)
            {
                return new ApiResponseDto<IssueDto>
                {
                    Success = false,
                    Message = "Issue não encontrada"
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

            // Atualizar propriedades não nulas
            if (!string.IsNullOrEmpty(dto.Title))
                issue.Title = dto.Title;

            if (!string.IsNullOrEmpty(dto.Description))
                issue.Description = dto.Description;

            if (dto.Type.HasValue)
                issue.Type = dto.Type.Value;

            if (dto.Priority.HasValue)
                issue.Priority = dto.Priority.Value;

            if (dto.Status.HasValue)
            {
                issue.Status = dto.Status.Value;
                if (dto.Status.Value == IssueStatus.Resolved)
                {
                    issue.ResolvedAt = DateTime.UtcNow;
                }
            }

            if (dto.AssignedTo != null)
                issue.AssignedTo = dto.AssignedTo;

            if (dto.Environment != null)
                issue.Environment = dto.Environment;

            if (dto.StepsToReproduce != null)
                issue.StepsToReproduce = dto.StepsToReproduce;

            if (dto.ExpectedResult != null)
                issue.ExpectedResult = dto.ExpectedResult;

            if (dto.ActualResult != null)
                issue.ActualResult = dto.ActualResult;

            if (dto.Resolution != null)
                issue.Resolution = dto.Resolution;

            if (dto.UserStoryId != issue.UserStoryId)
                issue.UserStoryId = dto.UserStoryId;

            issue.UpdatedBy = currentUser;
            issue.UpdatedAt = DateTime.UtcNow;

            await _issueRepository.UpdateAsync(issue);
            await _issueRepository.SaveChangesAsync();

            var updatedIssue = await _issueRepository.GetWithAttachmentsAsync(id);

            return new ApiResponseDto<IssueDto>
            {
                Success = true,
                Message = "Issue atualizada com sucesso",
                Data = await MapToDtoAsync(updatedIssue!)
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
    /// Exclui uma issue
    /// </summary>
    public async Task<ApiResponseDto<bool>> DeleteAsync(Guid id)
    {
        try
        {
            var issue = await _issueRepository.GetByIdAsync(id);

            if (issue == null)
            {
                return new ApiResponseDto<bool>
                {
                    Success = false,
                    Message = "Issue não encontrada"
                };
            }

            await _issueRepository.DeleteAsync(id);
            await _issueRepository.SaveChangesAsync();

            return new ApiResponseDto<bool>
            {
                Success = true,
                Message = "Issue excluída com sucesso",
                Data = true
            };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<bool>
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
    private async Task<IssueDto> MapToDtoAsync(Issue issue)
    {
        UserStorySummaryDto? userStorySummary = null;
        if (issue.UserStory != null)
        {
            userStorySummary = new UserStorySummaryDto
            {
                Id = issue.UserStory.Id,
                DemandNumber = issue.UserStory.DemandNumber,
                Title = issue.UserStory.Title,
                Status = issue.UserStory.Status,
                Priority = issue.UserStory.Priority,
                CreatedAt = issue.UserStory.CreatedAt,
                CreatedBy = issue.UserStory.CreatedBy,
                AttachmentsCount = issue.UserStory.Attachments.Count
            };
        }

        return new IssueDto
        {
            Id = issue.Id,
            IssueNumber = issue.IssueNumber,
            Title = issue.Title,
            Description = issue.Description,
            Type = issue.Type,
            Priority = issue.Priority,
            Status = issue.Status,
            AssignedTo = issue.AssignedTo,
            Reporter = issue.Reporter,
            Environment = issue.Environment,
            StepsToReproduce = issue.StepsToReproduce,
            ExpectedResult = issue.ExpectedResult,
            ActualResult = issue.ActualResult,
            Resolution = issue.Resolution,
            CreatedAt = issue.CreatedAt,
            UpdatedAt = issue.UpdatedAt,
            ResolvedAt = issue.ResolvedAt,
            CreatedBy = issue.CreatedBy,
            UpdatedBy = issue.UpdatedBy,
            UserStoryId = issue.UserStoryId,
            UserStory = userStorySummary,
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

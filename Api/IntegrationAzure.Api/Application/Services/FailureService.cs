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
    private readonly IIssueRepository _issueRepository;
    private readonly IUserStoryRepository _userStoryRepository;
    private readonly MarkdownGeneratorService _markdownGenerator;

    public FailureService(
        IFailureRepository failureRepository,
        IIssueRepository issueRepository,
        IUserStoryRepository userStoryRepository,
        MarkdownGeneratorService markdownGenerator)
    {
        _failureRepository = failureRepository ?? throw new ArgumentNullException(nameof(failureRepository));
        _issueRepository = issueRepository ?? throw new ArgumentNullException(nameof(issueRepository));
        _userStoryRepository = userStoryRepository ?? throw new ArgumentNullException(nameof(userStoryRepository));
        _markdownGenerator = markdownGenerator ?? throw new ArgumentNullException(nameof(markdownGenerator));
    }

    /// <summary>
    /// Cria uma nova falha
    /// </summary>
    public async Task<ApiResponseDto<FailureDto>> CreateAsync(CreateFailureDto dto, string currentUser)
    {
        try
        {
            // Verificar se já existe uma falha com o mesmo número
            var existingFailure = await _failureRepository.FirstOrDefaultAsync(f => f.FailureNumber == dto.FailureNumber);
            if (existingFailure != null)
            {
                return new ApiResponseDto<FailureDto>
                {
                    Success = false,
                    Message = "Já existe uma falha com este número",
                    Errors = new List<string> { "Número de falha duplicado" }
                };
            }

            // Verificar se a issue existe (se fornecida)
            if (dto.IssueId.HasValue)
            {
                var issue = await _issueRepository.GetByIdAsync(dto.IssueId.Value);
                if (issue == null)
                {
                    return new ApiResponseDto<FailureDto>
                    {
                        Success = false,
                        Message = "Issue não encontrada"
                    };
                }
            }

            // Verificar se a história de usuário existe (se fornecida)
            if (dto.UserStoryId.HasValue)
            {
                var userStory = await _userStoryRepository.GetByIdAsync(dto.UserStoryId.Value);
                if (userStory == null)
                {
                    return new ApiResponseDto<FailureDto>
                    {
                        Success = false,
                        Message = "História de usuário não encontrada"
                    };
                }
            }

            // Gerar a descrição em Markdown a partir dos dados estruturados
            var markdownDescription = _markdownGenerator.GenerateFailureDescription(dto);

            var failure = new Failure
            {
                FailureNumber = dto.FailureNumber,
                Title = dto.Title,
                Description = markdownDescription, // Descrição gerada em Markdown
                Severity = dto.Severity,
                OccurredAt = dto.OccurredAt,
                ReportedBy = dto.ReportedBy,
                Environment = dto.Environment,
                IssueId = dto.IssueId,
                UserStoryId = dto.UserStoryId,
                CreatedBy = currentUser,
                Status = FailureStatus.Reported
            };

            await _failureRepository.AddAsync(failure);
            await _failureRepository.SaveChangesAsync();

            var createdFailure = await _failureRepository.GetWithAttachmentsAsync(failure.Id);

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
                OccurredAt = f.OccurredAt,
                CreatedAt = f.CreatedAt,
                CreatedBy = f.CreatedBy,
                IssueTitle = f.Issue?.Title,
                UserStoryTitle = f.UserStory?.Title,
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
        IssueSummaryDto? issueSummary = null;
        if (failure.Issue != null)
        {
            issueSummary = new IssueSummaryDto
            {
                Id = failure.Issue.Id,
                IssueNumber = failure.Issue.IssueNumber,
                Title = failure.Issue.Title,
                Type = failure.Issue.Type,
                Priority = failure.Issue.Priority,
                Status = failure.Issue.Status,
                AssignedTo = failure.Issue.AssignedTo,
                CreatedAt = failure.Issue.CreatedAt,
                CreatedBy = failure.Issue.CreatedBy,
                UserStoryTitle = failure.Issue.UserStory?.Title,
                AttachmentsCount = failure.Issue.Attachments.Count
            };
        }

        UserStorySummaryDto? userStorySummary = null;
        if (failure.UserStory != null)
        {
            userStorySummary = new UserStorySummaryDto
            {
                Id = failure.UserStory.Id,
                DemandNumber = failure.UserStory.DemandNumber,
                Title = failure.UserStory.Title,
                Status = failure.UserStory.Status,
                Priority = failure.UserStory.Priority,
                CreatedAt = failure.UserStory.CreatedAt,
                CreatedBy = failure.UserStory.CreatedBy,
                AttachmentsCount = failure.UserStory.Attachments.Count
            };
        }

        return new FailureDto
        {
            Id = failure.Id,
            FailureNumber = failure.FailureNumber,
            Title = failure.Title,
            Description = failure.Description,
            Severity = failure.Severity,
            Status = failure.Status,
            OccurredAt = failure.OccurredAt,
            ReportedBy = failure.ReportedBy,
            Environment = failure.Environment,
            CreatedAt = failure.CreatedAt,
            CreatedBy = failure.CreatedBy,
            IssueId = failure.IssueId,
            Issue = issueSummary,
            UserStoryId = failure.UserStoryId,
            UserStory = userStorySummary,
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
}

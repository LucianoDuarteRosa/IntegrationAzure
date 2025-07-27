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

    public FailureService(
        IFailureRepository failureRepository,
        IIssueRepository issueRepository,
        IUserStoryRepository userStoryRepository)
    {
        _failureRepository = failureRepository ?? throw new ArgumentNullException(nameof(failureRepository));
        _issueRepository = issueRepository ?? throw new ArgumentNullException(nameof(issueRepository));
        _userStoryRepository = userStoryRepository ?? throw new ArgumentNullException(nameof(userStoryRepository));
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

            var failure = new Failure
            {
                FailureNumber = dto.FailureNumber,
                Title = dto.Title,
                Description = dto.Description,
                Severity = dto.Severity,
                OccurredAt = dto.OccurredAt,
                ReportedBy = dto.ReportedBy,
                AssignedTo = dto.AssignedTo,
                Environment = dto.Environment,
                SystemsAffected = dto.SystemsAffected,
                ImpactDescription = dto.ImpactDescription,
                StepsToReproduce = dto.StepsToReproduce,
                WorkaroundSolution = dto.WorkaroundSolution,
                IssueId = dto.IssueId,
                UserStoryId = dto.UserStoryId,
                EstimatedImpactCost = dto.EstimatedImpactCost,
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
                Data = await MapToDtoAsync(createdFailure!)
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
                Data = await MapToDtoAsync(failure)
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
                AssignedTo = f.AssignedTo,
                CreatedAt = f.CreatedAt,
                CreatedBy = f.CreatedBy,
                DowntimeDuration = f.DowntimeDuration,
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
    /// Atualiza uma falha
    /// </summary>
    public async Task<ApiResponseDto<FailureDto>> UpdateAsync(Guid id, UpdateFailureDto dto, string currentUser)
    {
        try
        {
            var failure = await _failureRepository.GetByIdAsync(id);

            if (failure == null)
            {
                return new ApiResponseDto<FailureDto>
                {
                    Success = false,
                    Message = "Falha não encontrada"
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

            // Atualizar propriedades não nulas
            if (!string.IsNullOrEmpty(dto.Title))
                failure.Title = dto.Title;

            if (!string.IsNullOrEmpty(dto.Description))
                failure.Description = dto.Description;

            if (dto.Severity.HasValue)
                failure.Severity = dto.Severity.Value;

            if (dto.Status.HasValue)
            {
                failure.Status = dto.Status.Value;
                if (dto.Status.Value == FailureStatus.Resolved)
                {
                    failure.ResolvedAt = DateTime.UtcNow;
                }
            }

            if (dto.AssignedTo != null)
                failure.AssignedTo = dto.AssignedTo;

            if (dto.Environment != null)
                failure.Environment = dto.Environment;

            if (dto.SystemsAffected != null)
                failure.SystemsAffected = dto.SystemsAffected;

            if (dto.ImpactDescription != null)
                failure.ImpactDescription = dto.ImpactDescription;

            if (dto.StepsToReproduce != null)
                failure.StepsToReproduce = dto.StepsToReproduce;

            if (dto.WorkaroundSolution != null)
                failure.WorkaroundSolution = dto.WorkaroundSolution;

            if (dto.RootCauseAnalysis != null)
                failure.RootCauseAnalysis = dto.RootCauseAnalysis;

            if (dto.PermanentSolution != null)
                failure.PermanentSolution = dto.PermanentSolution;

            if (dto.DowntimeDuration.HasValue)
                failure.DowntimeDuration = dto.DowntimeDuration;

            if (dto.EstimatedImpactCost.HasValue)
                failure.EstimatedImpactCost = dto.EstimatedImpactCost;

            if (dto.IssueId != failure.IssueId)
                failure.IssueId = dto.IssueId;

            if (dto.UserStoryId != failure.UserStoryId)
                failure.UserStoryId = dto.UserStoryId;

            failure.UpdatedBy = currentUser;
            failure.UpdatedAt = DateTime.UtcNow;

            await _failureRepository.UpdateAsync(failure);
            await _failureRepository.SaveChangesAsync();

            var updatedFailure = await _failureRepository.GetWithAttachmentsAsync(id);

            return new ApiResponseDto<FailureDto>
            {
                Success = true,
                Message = "Falha atualizada com sucesso",
                Data = await MapToDtoAsync(updatedFailure!)
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
    /// Exclui uma falha
    /// </summary>
    public async Task<ApiResponseDto<bool>> DeleteAsync(Guid id)
    {
        try
        {
            var failure = await _failureRepository.GetByIdAsync(id);

            if (failure == null)
            {
                return new ApiResponseDto<bool>
                {
                    Success = false,
                    Message = "Falha não encontrada"
                };
            }

            await _failureRepository.DeleteAsync(id);
            await _failureRepository.SaveChangesAsync();

            return new ApiResponseDto<bool>
            {
                Success = true,
                Message = "Falha excluída com sucesso",
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
    private async Task<FailureDto> MapToDtoAsync(Failure failure)
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
                TestCasesCount = failure.UserStory.TestCases.Count,
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
            AssignedTo = failure.AssignedTo,
            Environment = failure.Environment,
            SystemsAffected = failure.SystemsAffected,
            ImpactDescription = failure.ImpactDescription,
            StepsToReproduce = failure.StepsToReproduce,
            WorkaroundSolution = failure.WorkaroundSolution,
            RootCauseAnalysis = failure.RootCauseAnalysis,
            PermanentSolution = failure.PermanentSolution,
            CreatedAt = failure.CreatedAt,
            UpdatedAt = failure.UpdatedAt,
            ResolvedAt = failure.ResolvedAt,
            CreatedBy = failure.CreatedBy,
            UpdatedBy = failure.UpdatedBy,
            DowntimeDuration = failure.DowntimeDuration,
            EstimatedImpactCost = failure.EstimatedImpactCost,
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

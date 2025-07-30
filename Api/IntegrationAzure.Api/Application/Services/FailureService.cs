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
    private readonly MarkdownGeneratorService _markdownGenerator;

    public FailureService(
        IFailureRepository failureRepository,
        IUserStoryRepository userStoryRepository,
        MarkdownGeneratorService markdownGenerator)
    {
        _failureRepository = failureRepository ?? throw new ArgumentNullException(nameof(failureRepository));
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
            // Gerar a descrição em Markdown a partir dos dados estruturados
            var markdownDescription = _markdownGenerator.GenerateFailureDescription(dto, dto.Scenarios, dto.Observations);

            var failure = new Failure
            {
                FailureNumber = dto.FailureNumber,
                Title = dto.Title,
                Description = markdownDescription, // Descrição gerada em Markdown
                Severity = dto.Severity,
                OccurrenceType = (int)dto.OccurrenceType,
                OccurredAt = dto.OccurredAt,
                Environment = dto.Environment,
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
                OccurrenceType = f.OccurrenceType,
                OccurredAt = f.OccurredAt,
                CreatedAt = f.CreatedAt,
                CreatedBy = f.CreatedBy,
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
            OccurrenceType = (int)failure.OccurrenceType,
            OccurredAt = failure.OccurredAt,
            Environment = failure.Environment,
            CreatedAt = failure.CreatedAt,
            CreatedBy = failure.CreatedBy,
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

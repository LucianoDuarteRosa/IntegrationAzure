using IntegrationAzure.Api.Application.DTOs;
using IntegrationAzure.Api.Domain.Entities;
using IntegrationAzure.Api.Domain.Interfaces;

namespace IntegrationAzure.Api.Application.Services;

/// <summary>
/// Serviço para operações relacionadas a histórias de usuário
/// Implementa regras de negócio e orchestração entre domínio e infraestrutura
/// </summary>
public class UserStoryService
{
    private readonly IUserStoryRepository _userStoryRepository;
    private readonly IRepository<TestCase> _testCaseRepository;

    public UserStoryService(
        IUserStoryRepository userStoryRepository,
        IRepository<TestCase> testCaseRepository)
    {
        _userStoryRepository = userStoryRepository ?? throw new ArgumentNullException(nameof(userStoryRepository));
        _testCaseRepository = testCaseRepository ?? throw new ArgumentNullException(nameof(testCaseRepository));
    }

    /// <summary>
    /// Cria uma nova história de usuário com casos de teste
    /// </summary>
    public async Task<ApiResponseDto<UserStoryDto>> CreateAsync(CreateUserStoryDto dto, string currentUser)
    {
        try
        {
            // Verificar se já existe uma história com o mesmo número de demanda
            var existingStories = await _userStoryRepository.GetByDemandNumberAsync(dto.DemandNumber);
            if (existingStories.Any())
            {
                return new ApiResponseDto<UserStoryDto>
                {
                    Success = false,
                    Message = "Já existe uma história com este número de demanda",
                    Errors = new List<string> { "Número de demanda duplicado" }
                };
            }

            // Criar a entidade UserStory
            var userStory = new UserStory
            {
                DemandNumber = dto.DemandNumber,
                Title = dto.Title,
                AcceptanceCriteria = dto.AcceptanceCriteria,
                Description = dto.Description,
                Priority = dto.Priority,
                CreatedBy = currentUser,
                Status = UserStoryStatus.New
            };

            // Adicionar a história
            await _userStoryRepository.AddAsync(userStory);
            await _userStoryRepository.SaveChangesAsync();

            // Adicionar casos de teste
            foreach (var testCaseDto in dto.TestCases)
            {
                var testCase = new TestCase
                {
                    Description = testCaseDto.Description,
                    OrderIndex = testCaseDto.OrderIndex,
                    UserStoryId = userStory.Id,
                    CreatedBy = currentUser,
                    Status = TestCaseStatus.Pending
                };

                await _testCaseRepository.AddAsync(testCase);
            }

            await _testCaseRepository.SaveChangesAsync();

            // Buscar a história completa para retorno
            var completeUserStory = await _userStoryRepository.GetCompleteAsync(userStory.Id);

            return new ApiResponseDto<UserStoryDto>
            {
                Success = true,
                Message = "História criada com sucesso",
                Data = MapToDto(completeUserStory!)
            };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<UserStoryDto>
            {
                Success = false,
                Message = "Erro interno do servidor",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    /// <summary>
    /// Obtém uma história de usuário por ID
    /// </summary>
    public async Task<ApiResponseDto<UserStoryDto>> GetByIdAsync(Guid id)
    {
        try
        {
            var userStory = await _userStoryRepository.GetCompleteAsync(id);

            if (userStory == null)
            {
                return new ApiResponseDto<UserStoryDto>
                {
                    Success = false,
                    Message = "História não encontrada"
                };
            }

            return new ApiResponseDto<UserStoryDto>
            {
                Success = true,
                Message = "História encontrada",
                Data = MapToDto(userStory)
            };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<UserStoryDto>
            {
                Success = false,
                Message = "Erro interno do servidor",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    /// <summary>
    /// Obtém todas as histórias de usuário
    /// </summary>
    public async Task<ApiResponseDto<List<UserStorySummaryDto>>> GetAllAsync()
    {
        try
        {
            var userStories = await _userStoryRepository.GetAllAsync();

            var summaryDtos = userStories.Select(us => new UserStorySummaryDto
            {
                Id = us.Id,
                DemandNumber = us.DemandNumber,
                Title = us.Title,
                Status = us.Status,
                Priority = us.Priority,
                CreatedAt = us.CreatedAt,
                CreatedBy = us.CreatedBy,
                TestCasesCount = us.TestCases.Count,
                AttachmentsCount = us.Attachments.Count
            }).ToList();

            return new ApiResponseDto<List<UserStorySummaryDto>>
            {
                Success = true,
                Message = "Histórias encontradas",
                Data = summaryDtos
            };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<List<UserStorySummaryDto>>
            {
                Success = false,
                Message = "Erro interno do servidor",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    /// <summary>
    /// Atualiza uma história de usuário
    /// </summary>
    public async Task<ApiResponseDto<UserStoryDto>> UpdateAsync(Guid id, UpdateUserStoryDto dto, string currentUser)
    {
        try
        {
            var userStory = await _userStoryRepository.GetWithTestCasesAsync(id);

            if (userStory == null)
            {
                return new ApiResponseDto<UserStoryDto>
                {
                    Success = false,
                    Message = "História não encontrada"
                };
            }

            // Atualizar propriedades não nulas
            if (!string.IsNullOrEmpty(dto.Title))
                userStory.Title = dto.Title;

            if (!string.IsNullOrEmpty(dto.AcceptanceCriteria))
                userStory.AcceptanceCriteria = dto.AcceptanceCriteria;

            if (!string.IsNullOrEmpty(dto.Description))
                userStory.Description = dto.Description;

            if (dto.Status.HasValue)
                userStory.Status = dto.Status.Value;

            if (dto.Priority.HasValue)
                userStory.Priority = dto.Priority.Value;

            userStory.UpdatedBy = currentUser;
            userStory.UpdatedAt = DateTime.UtcNow;

            // Atualizar casos de teste se fornecidos
            if (dto.TestCases != null)
            {
                // Remover casos existentes
                foreach (var existingTestCase in userStory.TestCases.ToList())
                {
                    await _testCaseRepository.DeleteAsync(existingTestCase.Id);
                }

                // Adicionar novos casos
                foreach (var testCaseDto in dto.TestCases)
                {
                    var testCase = new TestCase
                    {
                        Description = testCaseDto.Description,
                        OrderIndex = testCaseDto.OrderIndex,
                        UserStoryId = userStory.Id,
                        CreatedBy = currentUser,
                        Status = TestCaseStatus.Pending
                    };

                    await _testCaseRepository.AddAsync(testCase);
                }
            }

            await _userStoryRepository.UpdateAsync(userStory);
            await _userStoryRepository.SaveChangesAsync();

            var updatedUserStory = await _userStoryRepository.GetCompleteAsync(id);

            return new ApiResponseDto<UserStoryDto>
            {
                Success = true,
                Message = "História atualizada com sucesso",
                Data = MapToDto(updatedUserStory!)
            };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<UserStoryDto>
            {
                Success = false,
                Message = "Erro interno do servidor",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    /// <summary>
    /// Exclui uma história de usuário
    /// </summary>
    public async Task<ApiResponseDto<bool>> DeleteAsync(Guid id)
    {
        try
        {
            var userStory = await _userStoryRepository.GetByIdAsync(id);

            if (userStory == null)
            {
                return new ApiResponseDto<bool>
                {
                    Success = false,
                    Message = "História não encontrada"
                };
            }

            await _userStoryRepository.DeleteAsync(id);
            await _userStoryRepository.SaveChangesAsync();

            return new ApiResponseDto<bool>
            {
                Success = true,
                Message = "História excluída com sucesso",
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
    private static UserStoryDto MapToDto(UserStory userStory)
    {
        return new UserStoryDto
        {
            Id = userStory.Id,
            DemandNumber = userStory.DemandNumber,
            Title = userStory.Title,
            AcceptanceCriteria = userStory.AcceptanceCriteria,
            Description = userStory.Description,
            Status = userStory.Status,
            Priority = userStory.Priority,
            CreatedAt = userStory.CreatedAt,
            UpdatedAt = userStory.UpdatedAt,
            CreatedBy = userStory.CreatedBy,
            UpdatedBy = userStory.UpdatedBy,
            TestCases = userStory.TestCases.Select(tc => new TestCaseDto
            {
                Id = tc.Id,
                Description = tc.Description,
                OrderIndex = tc.OrderIndex,
                Status = tc.Status,
                Result = tc.Result,
                Notes = tc.Notes,
                CreatedAt = tc.CreatedAt,
                CreatedBy = tc.CreatedBy
            }).OrderBy(tc => tc.OrderIndex).ToList(),
            Attachments = userStory.Attachments.Select(a => new AttachmentDto
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

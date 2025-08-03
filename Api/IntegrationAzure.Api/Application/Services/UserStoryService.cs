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
    private readonly MarkdownGeneratorService _markdownGenerator;
    private readonly HtmlGeneratorService _htmlGenerator;
    private readonly AzureDevOpsService _azureDevOpsService;

    public UserStoryService(
        IUserStoryRepository userStoryRepository,
        MarkdownGeneratorService markdownGenerator,
        HtmlGeneratorService htmlGenerator,
        AzureDevOpsService azureDevOpsService)
    {
        _userStoryRepository = userStoryRepository ?? throw new ArgumentNullException(nameof(userStoryRepository));
        _markdownGenerator = markdownGenerator ?? throw new ArgumentNullException(nameof(markdownGenerator));
        _htmlGenerator = htmlGenerator ?? throw new ArgumentNullException(nameof(htmlGenerator));
        _azureDevOpsService = azureDevOpsService ?? throw new ArgumentNullException(nameof(azureDevOpsService));
    }

    /// <summary>
    /// Cria uma nova história de usuário
    /// </summary>
    public async Task<ApiResponseDto<UserStoryDto>> CreateAsync(CreateUserStoryDto dto, string currentUser)
    {
        try
        {
            // Gerar a descrição em Markdown a partir dos dados estruturados
            // var markdownDescription = _markdownGenerator.GenerateUserStoryDescription(dto);

            // Gerar a descrição em HTML a partir dos dados estruturados (para Azure DevOps Discussion)
            // SEM os critérios de aceite (eles vão para campo específico)
            var htmlDescription = _htmlGenerator.GenerateUserStoryDescription(dto, includeAcceptanceCriteria: false);

            // Criar a entidade UserStory
            var userStory = new UserStory
            {
                DemandNumber = dto.DemandNumber,
                Title = dto.Title,
                AcceptanceCriteria = dto.AcceptanceCriteria,
                Description = htmlDescription, // Descrição gerada em HTML
                Priority = dto.Priority,
                CreatedBy = currentUser,
                Status = UserStoryStatus.New
            };

            // Salvar no banco de dados local
            await _userStoryRepository.AddAsync(userStory);
            await _userStoryRepository.SaveChangesAsync();

            // Buscar a história completa para retorno
            var completeUserStory = await _userStoryRepository.GetCompleteAsync(userStory.Id);

            // Automaticamente criar no Azure DevOps após salvar localmente
            try
            {
                var azureProjects = await _azureDevOpsService.GetProjectsAsync();
                if (azureProjects?.Any() == true)
                {
                    // Usar o projeto especificado no DemandNumber ou o primeiro disponível
                    var targetProject = azureProjects.FirstOrDefault(p => p.Id == dto.DemandNumber || p.Name == dto.DemandNumber)
                                       ?? azureProjects.First();

                    // Preparar campos adicionais baseados na prioridade
                    var additionalFields = new Dictionary<string, object>
                    {
                        ["Microsoft.VSTS.Common.Priority"] = GetAzurePriority(dto.Priority),
                        ["System.AreaPath"] = targetProject.Name,
                        ["System.IterationPath"] = targetProject.Name,
                        ["Microsoft.VSTS.Common.AcceptanceCriteria"] = dto.AcceptanceCriteria ?? string.Empty
                    };

                    // Criar o work item no Azure DevOps
                    var azureWorkItem = await _azureDevOpsService.CreateWorkItemAsync(
                        targetProject.Id,
                        "User Story",
                        completeUserStory?.Title ?? dto.Title,
                        "História criada pela Integração Azure", // Descrição simples
                        additionalFields,
                        htmlDescription // HTML vai diretamente para o comentário (não do banco)
                    );

                    Console.WriteLine($"História de usuário '{completeUserStory?.Title}' criada com sucesso no Azure DevOps - ID: {azureWorkItem.Id}");
                }
                else
                {
                    Console.WriteLine("Nenhum projeto disponível no Azure DevOps para criar a User Story");
                }
            }
            catch (Exception azureEx)
            {
                // Se falhar a criação no Azure DevOps, logar o erro mas não falhar a operação local
                Console.WriteLine($"Erro ao criar história de usuário no Azure DevOps: {azureEx.Message}");
                // O usuário ainda tem a história salva localmente
            }

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
            CreatedBy = userStory.CreatedBy,
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

    /// <summary>
    /// Mapeia prioridade interna para prioridade do Azure DevOps
    /// </summary>
    private static int GetAzurePriority(Priority priority)
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
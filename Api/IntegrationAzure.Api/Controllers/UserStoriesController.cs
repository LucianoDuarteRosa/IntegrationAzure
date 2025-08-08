using Microsoft.AspNetCore.Mvc;
using IntegrationAzure.Api.Application.Services;
using IntegrationAzure.Api.Application.DTOs;
using FluentValidation;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace IntegrationAzure.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserStoriesController : BaseController
    {
        private readonly UserStoryService _userStoryService;
        private readonly LogService _logService;
        private readonly IValidator<CreateUserStoryDto>? _createValidator;

        public UserStoriesController(
            UserStoryService userStoryService,
            LogService logService,
            IValidator<CreateUserStoryDto>? createValidator = null)
        {
            _userStoryService = userStoryService;
            _logService = logService;
            _createValidator = createValidator;
        }

        /// <summary>
        /// Cria uma nova história de usuário
        /// </summary>
        /// <param name="createDto">Dados da história de usuário a ser criada</param>
        /// <returns>História de usuário criada</returns>
        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<UserStoryDto>>> Create([FromBody][Required] CreateUserStoryDto createDto)
        {
            try
            {
                // Verificar se o ModelState é válido
                if (!ModelState.IsValid)
                {
                    return ValidationErrorResponse<UserStoryDto>(ModelState);
                }

                if (createDto == null)
                {
                    return ErrorResponse<UserStoryDto>(
                        "Dados de entrada inválidos",
                        new List<string> { "O objeto createDto é obrigatório" },
                        400
                    );
                }

                // Validação manual se o validador estiver disponível
                if (_createValidator != null)
                {
                    var validationResult = await _createValidator.ValidateAsync(createDto);
                    if (!validationResult.IsValid)
                    {
                        var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();

                        // Log de erro de validação
                        await _logService.LogActionAsync(
                            "CREATE_FAILED",
                            "UserStory",
                            null,
                            GetCurrentUser(),
                            $"Validation errors: {string.Join(", ", errors)}",
                            Domain.Entities.LogLevel.Warning
                        );

                        return ErrorResponse<UserStoryDto>(
                            "Dados de entrada inválidos",
                            errors,
                            400
                        );
                    }
                }

                var currentUser = GetCurrentUser();
                var result = await _userStoryService.CreateAsync(createDto, currentUser);

                if (result.Success && result.Data != null)
                {
                    // Log de sucesso
                    await _logService.LogActionAsync(
                        "CREATE_SUCCESS",
                        "UserStory",
                        result.Data.Id.ToString(),
                        currentUser,
                        $"Created: {result.Data.Title} (#{result.Data.DemandNumber}) - Local database and Azure DevOps integration",
                        Domain.Entities.LogLevel.Success
                    );

                    return SuccessResponse(result.Data, "História de usuário criada com sucesso");
                }

                // Log de erro do serviço
                await _logService.LogActionAsync(
                    "CREATE_FAILED",
                    "UserStory",
                    null,
                    currentUser,
                    result.Message,
                    Domain.Entities.LogLevel.Error
                );

                return ProcessServiceResponse(result);
            }
            catch (Exception ex)
            {
                // Log de erro crítico
                await _logService.LogActionAsync(
                    "CREATE_ERROR",
                    "UserStory",
                    null,
                    GetCurrentUser(),
                    $"Exception: {ex.Message}",
                    Domain.Entities.LogLevel.Error
                );

                return ErrorResponse<UserStoryDto>(
                    "Erro interno do servidor",
                    new List<string> { ex.Message },
                    500
                );
            }
        }

        /// <summary>
        /// Cria uma nova história de usuário com anexos
        /// </summary>
        /// <param name="jsonData">Dados JSON da história de usuário</param>
        /// <param name="files">Arquivos anexos</param>
        /// <returns>História de usuário criada</returns>
        [HttpPost("with-attachments")]
        public async Task<ActionResult<ApiResponseDto<UserStoryDto>>> CreateWithAttachments(
            [FromForm] string jsonData,
            [FromForm] ICollection<IFormFile>? files)
        {
            try
            {
                // Deserializar os dados JSON
                var createDto = System.Text.Json.JsonSerializer.Deserialize<CreateUserStoryDto>(jsonData, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (createDto == null)
                {
                    return ErrorResponse<UserStoryDto>(
                        "Dados de entrada inválidos",
                        new List<string> { "O objeto createDto é obrigatório" },
                        400
                    );
                }

                // Criar a história de usuário com anexos
                var result = await _userStoryService.CreateWithAttachmentsAsync(createDto, files, GetCurrentUser());
                return Ok(result);
            }
            catch (Exception ex)
            {
                return ErrorResponse<UserStoryDto>($"Erro ao criar história de usuário com anexos: {ex.Message}", null, 500);
            }
        }
    }
}
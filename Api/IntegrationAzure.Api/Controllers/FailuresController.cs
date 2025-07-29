using Microsoft.AspNetCore.Mvc;
using IntegrationAzure.Api.Application.DTOs;
using IntegrationAzure.Api.Application.Services;
using IntegrationAzure.Api.Application.Validators;
using FluentValidation;

namespace IntegrationAzure.Api.Controllers;

/// <summary>
/// Controller para operações com falhas
/// Implementa endpoints RESTful seguindo boas práticas
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class FailuresController : BaseController
{
    private readonly FailureService _failureService;
    private readonly LogService _logService;
    private readonly IValidator<CreateFailureDto> _createValidator;

    public FailuresController(
        FailureService failureService,
        LogService logService,
        IValidator<CreateFailureDto> createValidator)
    {
        _failureService = failureService ?? throw new ArgumentNullException(nameof(failureService));
        _logService = logService ?? throw new ArgumentNullException(nameof(logService));
        _createValidator = createValidator ?? throw new ArgumentNullException(nameof(createValidator));
    }

    /// <summary>
    /// Obtém todas as falhas
    /// </summary>
    /// <returns>Lista de falhas</returns>
    /// <response code="200">Falhas encontradas com sucesso</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseDto<List<FailureSummaryDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 500)]
    public async Task<ActionResult<ApiResponseDto<List<FailureSummaryDto>>>> GetAll()
    {
        try
        {
            var result = await _failureService.GetAllAsync();

            if (result.Success)
            {
                await _logService.LogActionAsync(
                    "GET_ALL",
                    "Failure",
                    null,
                    GetCurrentUser(),
                    $"Retrieved {result.Data?.Count ?? 0} failures",
                    Domain.Entities.LogLevel.Info
                );
            }

            return ProcessServiceResponse(result);
        }
        catch (Exception ex)
        {
            await _logService.LogActionAsync(
                "GET_ALL_ERROR",
                "Failure",
                null,
                GetCurrentUser(),
                $"Exception: {ex.Message}",
                Domain.Entities.LogLevel.Error
            );
            throw;
        }
    }

    /// <summary>
    /// Obtém uma falha específica por ID
    /// </summary>
    /// <param name="id">ID da falha</param>
    /// <returns>Falha encontrada</returns>
    /// <response code="200">Falha encontrada com sucesso</response>
    /// <response code="404">Falha não encontrada</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponseDto<FailureDto>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 404)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 500)]
    public async Task<ActionResult<ApiResponseDto<FailureDto>>> GetById(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
            {
                await _logService.LogActionAsync(
                    "GET_BY_ID_FAILED",
                    "Failure",
                    id.ToString(),
                    GetCurrentUser(),
                    "Invalid ID provided",
                    Domain.Entities.LogLevel.Warning
                );

                return ErrorResponse<FailureDto>("ID inválido");
            }

            var result = await _failureService.GetByIdAsync(id);

            if (result.Success && result.Data != null)
            {
                await _logService.LogActionAsync(
                    "GET_BY_ID",
                    "Failure",
                    id.ToString(),
                    GetCurrentUser(),
                    $"Retrieved failure: {result.Data.Title}",
                    Domain.Entities.LogLevel.Info
                );
            }
            else
            {
                await _logService.LogActionAsync(
                    "GET_BY_ID_NOT_FOUND",
                    "Failure",
                    id.ToString(),
                    GetCurrentUser(),
                    "Failure not found",
                    Domain.Entities.LogLevel.Warning
                );
            }

            return ProcessServiceResponse(result);
        }
        catch (Exception ex)
        {
            await _logService.LogActionAsync(
                "GET_BY_ID_ERROR",
                "Failure",
                id.ToString(),
                GetCurrentUser(),
                $"Exception: {ex.Message}",
                Domain.Entities.LogLevel.Error
            );
            throw;
        }
    }

    /// <summary>
    /// Cria uma nova falha
    /// </summary>
    /// <param name="dto">Dados da nova falha</param>
    /// <returns>Falha criada</returns>
    /// <response code="201">Falha criada com sucesso</response>
    /// <response code="400">Dados de entrada inválidos</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseDto<FailureDto>), 201)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 400)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 500)]
    public async Task<ActionResult<ApiResponseDto<FailureDto>>> Create([FromBody] CreateFailureDto dto)
    {
        try
        {
            // Validação dos dados de entrada
            var validationResult = await _createValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();

                await _logService.LogActionAsync(
                    "CREATE_FAILED",
                    "Failure",
                    null,
                    GetCurrentUser(),
                    $"Validation errors: {string.Join(", ", errors)}",
                    Domain.Entities.LogLevel.Warning
                );

                return ErrorResponse<FailureDto>("Dados de entrada inválidos", errors);
            }

            var currentUser = GetCurrentUser();
            var result = await _failureService.CreateAsync(dto, currentUser);

            if (result.Success && result.Data != null)
            {
                await _logService.LogActionAsync(
                    "CREATE_SUCCESS",
                    "Failure",
                    result.Data.Id.ToString(),
                    currentUser,
                    $"Created failure: {result.Data.Title}",
                    Domain.Entities.LogLevel.Success
                );

                return Created($"/api/failures/{result.Data.Id}", result);
            }

            await _logService.LogActionAsync(
                "CREATE_FAILED",
                "Failure",
                null,
                currentUser,
                result.Message,
                Domain.Entities.LogLevel.Error
            );

            return ProcessServiceResponse(result);
        }
        catch (Exception ex)
        {
            await _logService.LogActionAsync(
                "CREATE_ERROR",
                "Failure",
                null,
                GetCurrentUser(),
                $"Exception: {ex.Message}",
                Domain.Entities.LogLevel.Error
            );
            throw;
        }
    }
}

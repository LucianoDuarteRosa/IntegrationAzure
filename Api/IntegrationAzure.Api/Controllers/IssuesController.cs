using Microsoft.AspNetCore.Mvc;
using IntegrationAzure.Api.Application.DTOs;
using IntegrationAzure.Api.Application.Services;
using IntegrationAzure.Api.Application.Validators;
using IntegrationAzure.Api.Domain.Extensions;
using FluentValidation;

namespace IntegrationAzure.Api.Controllers;

/// <summary>
/// Controller para operações com issues
/// Implementa endpoints RESTful seguindo boas práticas
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class IssuesController : BaseController
{
    private readonly IssueService _issueService;
    private readonly LogService _logService;
    private readonly IValidator<CreateIssueDto> _createValidator;

    public IssuesController(
        IssueService issueService,
        LogService logService,
        IValidator<CreateIssueDto> createValidator)
    {
        _issueService = issueService ?? throw new ArgumentNullException(nameof(issueService));
        _logService = logService ?? throw new ArgumentNullException(nameof(logService));
        _createValidator = createValidator ?? throw new ArgumentNullException(nameof(createValidator));
    }

    /// <summary>
    /// Obtém todas as issues
    /// </summary>
    /// <returns>Lista de issues</returns>
    /// <response code="200">Issues encontradas com sucesso</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseDto<List<IssueSummaryDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 500)]
    public async Task<ActionResult<ApiResponseDto<List<IssueSummaryDto>>>> GetAll()
    {
        try
        {
            var result = await _issueService.GetAllAsync();

            if (result.Success)
            {
                await _logService.LogActionAsync(
                    "GET_ALL",
                    "Issue",
                    null,
                    GetCurrentUser(),
                    $"Retrieved {result.Data?.Count ?? 0} issues",
                    Domain.Entities.LogLevel.Info
                );
            }

            return ProcessServiceResponse(result);
        }
        catch (Exception ex)
        {
            await _logService.LogActionAsync(
                "GET_ALL_ERROR",
                "Issue",
                null,
                GetCurrentUser(),
                $"Exception: {ex.Message}",
                Domain.Entities.LogLevel.Error
            );
            throw;
        }
    }

    /// <summary>
    /// Obtém uma issue específica por ID
    /// </summary>
    /// <param name="id">ID da issue</param>
    /// <returns>Issue encontrada</returns>
    /// <response code="200">Issue encontrada com sucesso</response>
    /// <response code="404">Issue não encontrada</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponseDto<IssueDto>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 404)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 500)]
    public async Task<ActionResult<ApiResponseDto<IssueDto>>> GetById(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
            {
                await _logService.LogActionAsync(
                    "GET_BY_ID_FAILED",
                    "Issue",
                    id.ToString(),
                    GetCurrentUser(),
                    "Invalid ID provided",
                    Domain.Entities.LogLevel.Warning
                );

                return ErrorResponse<IssueDto>("ID inválido");
            }

            var result = await _issueService.GetByIdAsync(id);

            if (result.Success && result.Data != null)
            {
                await _logService.LogActionAsync(
                    "GET_BY_ID",
                    "Issue",
                    id.ToString(),
                    GetCurrentUser(),
                    $"Retrieved issue: {result.Data.Title}",
                    Domain.Entities.LogLevel.Info
                );
            }
            else
            {
                await _logService.LogActionAsync(
                    "GET_BY_ID_NOT_FOUND",
                    "Issue",
                    id.ToString(),
                    GetCurrentUser(),
                    "Issue not found",
                    Domain.Entities.LogLevel.Warning
                );
            }

            return ProcessServiceResponse(result);
        }
        catch (Exception ex)
        {
            await _logService.LogActionAsync(
                "GET_BY_ID_ERROR",
                "Issue",
                id.ToString(),
                GetCurrentUser(),
                $"Exception: {ex.Message}",
                Domain.Entities.LogLevel.Error
            );
            throw;
        }
    }

    /// <summary>
    /// Cria uma nova issue
    /// </summary>
    /// <param name="dto">Dados da nova issue</param>
    /// <returns>Issue criada</returns>
    /// <response code="201">Issue criada com sucesso</response>
    /// <response code="400">Dados de entrada inválidos</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseDto<IssueDto>), 201)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 400)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 500)]
    public async Task<ActionResult<ApiResponseDto<IssueDto>>> Create([FromBody] CreateIssueDto dto)
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
                    "Issue",
                    null,
                    GetCurrentUser(),
                    $"Validation errors: {string.Join(", ", errors)}",
                    Domain.Entities.LogLevel.Warning
                );

                return ErrorResponse<IssueDto>("Dados de entrada inválidos", errors);
            }

            var currentUser = GetCurrentUser();
            var result = await _issueService.CreateAsync(dto, currentUser);

            if (result.Success && result.Data != null)
            {
                await _logService.LogActionAsync(
                    "CREATE_SUCCESS",
                    "Issue",
                    result.Data.Id.ToString(),
                    currentUser,
                    $"Created issue: {result.Data.Title}",
                    Domain.Entities.LogLevel.Success
                );

                return Created($"/api/issues/{result.Data.Id}", result);
            }

            await _logService.LogActionAsync(
                "CREATE_FAILED",
                "Issue",
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
                "Issue",
                null,
                GetCurrentUser(),
                $"Exception: {ex.Message}",
                Domain.Entities.LogLevel.Error
            );
            throw;
        }
    }
}

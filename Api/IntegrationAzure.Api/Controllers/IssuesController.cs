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

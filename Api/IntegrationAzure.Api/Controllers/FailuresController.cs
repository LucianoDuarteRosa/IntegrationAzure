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
    private readonly IValidator<CreateFailureDto> _createValidator;

    public FailuresController(
        FailureService failureService,
        IValidator<CreateFailureDto> createValidator)
    {
        _failureService = failureService ?? throw new ArgumentNullException(nameof(failureService));
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
        var result = await _failureService.GetAllAsync();
        return ProcessServiceResponse(result);
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
        if (id == Guid.Empty)
        {
            return ErrorResponse<FailureDto>("ID inválido");
        }

        var result = await _failureService.GetByIdAsync(id);
        return ProcessServiceResponse(result);
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
        // Validação dos dados de entrada
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ErrorResponse<FailureDto>("Dados de entrada inválidos", errors);
        }

        var currentUser = GetCurrentUser();
        var result = await _failureService.CreateAsync(dto, currentUser);

        if (result.Success)
        {
            return Created($"/api/failures/{result.Data!.Id}", result);
        }

        return ProcessServiceResponse(result);
    }
}

using Microsoft.AspNetCore.Mvc;
using IntegrationAzure.Api.Application.DTOs;
using IntegrationAzure.Api.Application.Services;
using IntegrationAzure.Api.Application.Validators;
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
    private readonly IValidator<CreateIssueDto> _createValidator;
    private readonly IValidator<UpdateIssueDto> _updateValidator;

    public IssuesController(
        IssueService issueService,
        IValidator<CreateIssueDto> createValidator,
        IValidator<UpdateIssueDto> updateValidator)
    {
        _issueService = issueService ?? throw new ArgumentNullException(nameof(issueService));
        _createValidator = createValidator ?? throw new ArgumentNullException(nameof(createValidator));
        _updateValidator = updateValidator ?? throw new ArgumentNullException(nameof(updateValidator));
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
        var result = await _issueService.GetAllAsync();
        return ProcessServiceResponse(result);
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
        if (id == Guid.Empty)
        {
            return ErrorResponse<IssueDto>("ID inválido");
        }

        var result = await _issueService.GetByIdAsync(id);
        return ProcessServiceResponse(result);
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
        // Validação dos dados de entrada
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ErrorResponse<IssueDto>("Dados de entrada inválidos", errors);
        }

        var currentUser = GetCurrentUser();
        var result = await _issueService.CreateAsync(dto, currentUser);

        if (result.Success)
        {
            return Created($"/api/issues/{result.Data!.Id}", result);
        }

        return ProcessServiceResponse(result);
    }

    /// <summary>
    /// Atualiza uma issue existente
    /// </summary>
    /// <param name="id">ID da issue a ser atualizada</param>
    /// <param name="dto">Dados para atualização</param>
    /// <returns>Issue atualizada</returns>
    /// <response code="200">Issue atualizada com sucesso</response>
    /// <response code="400">Dados de entrada inválidos</response>
    /// <response code="404">Issue não encontrada</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponseDto<IssueDto>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 400)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 404)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 500)]
    public async Task<ActionResult<ApiResponseDto<IssueDto>>> Update(Guid id, [FromBody] UpdateIssueDto dto)
    {
        if (id == Guid.Empty)
        {
            return ErrorResponse<IssueDto>("ID inválido");
        }

        // Validação dos dados de entrada
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ErrorResponse<IssueDto>("Dados de entrada inválidos", errors);
        }

        var currentUser = GetCurrentUser();
        var result = await _issueService.UpdateAsync(id, dto, currentUser);

        return ProcessServiceResponse(result);
    }

    /// <summary>
    /// Exclui uma issue
    /// </summary>
    /// <param name="id">ID da issue a ser excluída</param>
    /// <returns>Confirmação da exclusão</returns>
    /// <response code="200">Issue excluída com sucesso</response>
    /// <response code="404">Issue não encontrada</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponseDto<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 404)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 500)]
    public async Task<ActionResult<ApiResponseDto<bool>>> Delete(Guid id)
    {
        if (id == Guid.Empty)
        {
            return ErrorResponse<bool>("ID inválido");
        }

        var result = await _issueService.DeleteAsync(id);
        return ProcessServiceResponse(result);
    }
}

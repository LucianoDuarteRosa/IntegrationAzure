using Microsoft.AspNetCore.Mvc;
using IntegrationAzure.Api.Application.DTOs;
using IntegrationAzure.Api.Application.Services;
using IntegrationAzure.Api.Application.Validators;
using FluentValidation;

namespace IntegrationAzure.Api.Controllers;

/// <summary>
/// Controller para operações com histórias de usuário
/// Implementa endpoints RESTful seguindo boas práticas
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UserStoriesController : BaseController
{
    private readonly UserStoryService _userStoryService;
    private readonly IValidator<CreateUserStoryDto> _createValidator;
    private readonly IValidator<UpdateUserStoryDto> _updateValidator;

    public UserStoriesController(
        UserStoryService userStoryService,
        IValidator<CreateUserStoryDto> createValidator,
        IValidator<UpdateUserStoryDto> updateValidator)
    {
        _userStoryService = userStoryService ?? throw new ArgumentNullException(nameof(userStoryService));
        _createValidator = createValidator ?? throw new ArgumentNullException(nameof(createValidator));
        _updateValidator = updateValidator ?? throw new ArgumentNullException(nameof(updateValidator));
    }

    /// <summary>
    /// Obtém todas as histórias de usuário
    /// </summary>
    /// <returns>Lista de histórias de usuário</returns>
    /// <response code="200">Histórias encontradas com sucesso</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseDto<List<UserStorySummaryDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 500)]
    public async Task<ActionResult<ApiResponseDto<List<UserStorySummaryDto>>>> GetAll()
    {
        var result = await _userStoryService.GetAllAsync();
        return ProcessServiceResponse(result);
    }

    /// <summary>
    /// Obtém uma história de usuário específica por ID
    /// </summary>
    /// <param name="id">ID da história de usuário</param>
    /// <returns>História de usuário encontrada</returns>
    /// <response code="200">História encontrada com sucesso</response>
    /// <response code="404">História não encontrada</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponseDto<UserStoryDto>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 404)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 500)]
    public async Task<ActionResult<ApiResponseDto<UserStoryDto>>> GetById(Guid id)
    {
        if (id == Guid.Empty)
        {
            return ErrorResponse<UserStoryDto>("ID inválido");
        }

        var result = await _userStoryService.GetByIdAsync(id);
        return ProcessServiceResponse(result);
    }

    /// <summary>
    /// Cria uma nova história de usuário
    /// </summary>
    /// <param name="dto">Dados da nova história</param>
    /// <returns>História criada</returns>
    /// <response code="201">História criada com sucesso</response>
    /// <response code="400">Dados de entrada inválidos</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseDto<UserStoryDto>), 201)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 400)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 500)]
    public async Task<ActionResult<ApiResponseDto<UserStoryDto>>> Create([FromBody] CreateUserStoryDto dto)
    {
        // Validação dos dados de entrada
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ErrorResponse<UserStoryDto>("Dados de entrada inválidos", errors);
        }

        var currentUser = GetCurrentUser();
        var result = await _userStoryService.CreateAsync(dto, currentUser);

        if (result.Success)
        {
            return Created($"/api/userstories/{result.Data!.Id}", result);
        }

        return ProcessServiceResponse(result);
    }

    /// <summary>
    /// Atualiza uma história de usuário existente
    /// </summary>
    /// <param name="id">ID da história a ser atualizada</param>
    /// <param name="dto">Dados para atualização</param>
    /// <returns>História atualizada</returns>
    /// <response code="200">História atualizada com sucesso</response>
    /// <response code="400">Dados de entrada inválidos</response>
    /// <response code="404">História não encontrada</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponseDto<UserStoryDto>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 400)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 404)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 500)]
    public async Task<ActionResult<ApiResponseDto<UserStoryDto>>> Update(Guid id, [FromBody] UpdateUserStoryDto dto)
    {
        if (id == Guid.Empty)
        {
            return ErrorResponse<UserStoryDto>("ID inválido");
        }

        // Validação dos dados de entrada
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ErrorResponse<UserStoryDto>("Dados de entrada inválidos", errors);
        }

        var currentUser = GetCurrentUser();
        var result = await _userStoryService.UpdateAsync(id, dto, currentUser);

        return ProcessServiceResponse(result);
    }

    /// <summary>
    /// Exclui uma história de usuário
    /// </summary>
    /// <param name="id">ID da história a ser excluída</param>
    /// <returns>Confirmação da exclusão</returns>
    /// <response code="200">História excluída com sucesso</response>
    /// <response code="404">História não encontrada</response>
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

        var result = await _userStoryService.DeleteAsync(id);
        return ProcessServiceResponse(result);
    }
}

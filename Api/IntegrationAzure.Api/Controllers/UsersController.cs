using Microsoft.AspNetCore.Mvc;
using IntegrationAzure.Api.Application.DTOs;
using IntegrationAzure.Api.Application.Services;

namespace IntegrationAzure.Api.Controllers;

/// <summary>
/// Controller para operações CRUD de usuários
/// Gerencia usuários do sistema com seus perfis e permissões
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UsersController : BaseController
{
    private readonly UserService _userService;

    public UsersController(UserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    /// <summary>
    /// Obtém todos os usuários
    /// </summary>
    /// <returns>Lista de usuários</returns>
    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<UserDto>>>> GetAll()
    {
        var result = await _userService.GetAllAsync();
        return ProcessServiceResponse(result);
    }

    /// <summary>
    /// Obtém um usuário por ID
    /// </summary>
    /// <param name="id">ID do usuário</param>
    /// <returns>Usuário encontrado</returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponseDto<UserDto>>> GetById(Guid id)
    {
        var result = await _userService.GetByIdAsync(id);
        return ProcessServiceResponse(result);
    }

    /// <summary>
    /// Obtém usuários por perfil
    /// </summary>
    /// <param name="profileId">ID do perfil</param>
    /// <returns>Lista de usuários do perfil</returns>
    [HttpGet("profile/{profileId:guid}")]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<SimpleUserDto>>>> GetByProfile(Guid profileId)
    {
        var result = await _userService.GetByProfileAsync(profileId);
        return ProcessServiceResponse(result);
    }

    /// <summary>
    /// Cria um novo usuário
    /// </summary>
    /// <param name="dto">Dados do usuário a ser criado</param>
    /// <returns>Usuário criado</returns>
    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<UserDto>>> Create([FromBody] CreateUserDto dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationErrorResponse<UserDto>(ModelState);
        }

        var currentUser = GetCurrentUser();
        var result = await _userService.CreateAsync(dto, currentUser);

        if (result.Success)
        {
            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);
        }

        return ProcessServiceResponse(result);
    }

    /// <summary>
    /// Atualiza um usuário existente
    /// </summary>
    /// <param name="id">ID do usuário</param>
    /// <param name="dto">Dados atualizados do usuário</param>
    /// <returns>Usuário atualizado</returns>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponseDto<UserDto>>> Update(Guid id, [FromBody] UpdateUserDto dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationErrorResponse<UserDto>(ModelState);
        }

        var currentUser = GetCurrentUser();
        var result = await _userService.UpdateAsync(id, dto, currentUser);
        return ProcessServiceResponse(result);
    }

    /// <summary>
    /// Remove um usuário (soft delete)
    /// </summary>
    /// <param name="id">ID do usuário</param>
    /// <returns>Resultado da operação</returns>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponseDto<bool>>> Delete(Guid id)
    {
        var currentUser = GetCurrentUser();
        var result = await _userService.DeleteAsync(id, currentUser);
        return ProcessServiceResponse(result);
    }

    /// <summary>
    /// Altera a senha de um usuário
    /// </summary>
    /// <param name="id">ID do usuário</param>
    /// <param name="dto">Dados da alteração de senha</param>
    /// <returns>Resultado da operação</returns>
    [HttpPatch("{id:guid}/change-password")]
    public async Task<ActionResult<ApiResponseDto<bool>>> ChangePassword(Guid id, [FromBody] ChangePasswordDto dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationErrorResponse<bool>(ModelState);
        }

        var currentUser = GetCurrentUser();
        var result = await _userService.ChangePasswordAsync(id, dto, currentUser);
        return ProcessServiceResponse(result);
    }

    /// <summary>
    /// Altera a senha de um usuário por hierarquia (sem senha atual)
    /// </summary>
    /// <param name="id">ID do usuário</param>
    /// <param name="dto">Dados da nova senha</param>
    /// <returns>Resultado da operação</returns>
    [HttpPatch("{id:guid}/admin-change-password")]
    public async Task<ActionResult<ApiResponseDto<bool>>> AdminChangePassword(Guid id, [FromBody] AdminChangePasswordDto dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationErrorResponse<bool>(ModelState);
        }

        var currentUser = GetCurrentUser();
        var result = await _userService.AdminChangePasswordAsync(id, dto, currentUser);
        return ProcessServiceResponse(result);
    }
}

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
    private readonly LogService _logService;

    public UsersController(UserService userService, LogService logService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _logService = logService ?? throw new ArgumentNullException(nameof(logService));
    }

    /// <summary>
    /// Obtém todos os usuários
    /// </summary>
    /// <returns>Lista de usuários</returns>
    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<UserDto>>>> GetAll()
    {
        try
        {
            var result = await _userService.GetAllAsync();
            return ProcessServiceResponse(result);
        }
        catch (Exception ex)
        {
            await _logService.LogActionAsync(
                "GET_ALL_ERROR",
                "User",
                null,
                GetCurrentUser(),
                $"Exception: {ex.Message}",
                Domain.Entities.LogLevel.Error
            );
            throw;
        }
    }

    /// <summary>
    /// Cria um novo usuário
    /// </summary>
    /// <param name="dto">Dados do usuário a ser criado</param>
    /// <returns>Usuário criado</returns>
    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<UserDto>>> Create([FromBody] CreateUserDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();

                await _logService.LogActionAsync(
                    "CREATE_FAILED",
                    "User",
                    null,
                    GetCurrentUser(),
                    $"Validation errors: {string.Join(", ", errors)}",
                    Domain.Entities.LogLevel.Warning
                );

                return ValidationErrorResponse<UserDto>(ModelState);
            }

            var currentUser = GetCurrentUser();
            var result = await _userService.CreateAsync(dto, currentUser);

            if (result.Success && result.Data != null)
            {
                await _logService.LogActionAsync(
                    "CREATE_SUCCESS",
                    "User",
                    result.Data.Id.ToString(),
                    currentUser,
                    $"Created user: {result.Data.Name} ({result.Data.Email})",
                    Domain.Entities.LogLevel.Success
                );

                return SuccessResponse(result.Data, "Usuário criado com sucesso");
            }

            await _logService.LogActionAsync(
                "CREATE_FAILED",
                "User",
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
                "User",
                null,
                GetCurrentUser(),
                $"Exception: {ex.Message}",
                Domain.Entities.LogLevel.Error
            );
            throw;
        }
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
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();

                await _logService.LogActionAsync(
                    "UPDATE_FAILED",
                    "User",
                    id.ToString(),
                    GetCurrentUser(),
                    $"Validation errors: {string.Join(", ", errors)}",
                    Domain.Entities.LogLevel.Warning
                );

                return ValidationErrorResponse<UserDto>(ModelState);
            }

            var currentUser = GetCurrentUser();
            var result = await _userService.UpdateAsync(id, dto, currentUser);

            if (result.Success && result.Data != null)
            {
                await _logService.LogActionAsync(
                    "UPDATE_SUCCESS",
                    "User",
                    id.ToString(),
                    currentUser,
                    $"Updated user: {result.Data.Name} ({result.Data.Email})",
                    Domain.Entities.LogLevel.Success
                );
            }
            else
            {
                await _logService.LogActionAsync(
                    "UPDATE_FAILED",
                    "User",
                    id.ToString(),
                    currentUser,
                    result.Message,
                    Domain.Entities.LogLevel.Error
                );
            }

            return ProcessServiceResponse(result);
        }
        catch (Exception ex)
        {
            await _logService.LogActionAsync(
                "UPDATE_ERROR",
                "User",
                id.ToString(),
                GetCurrentUser(),
                $"Exception: {ex.Message}",
                Domain.Entities.LogLevel.Error
            );
            throw;
        }
    }

    /// <summary>
    /// Remove um usuário (soft delete)
    /// </summary>
    /// <param name="id">ID do usuário</param>
    /// <returns>Resultado da operação</returns>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponseDto<bool>>> Delete(Guid id)
    {
        try
        {
            var currentUser = GetCurrentUser();
            var result = await _userService.DeleteAsync(id, currentUser);

            if (result.Success)
            {
                await _logService.LogActionAsync(
                    "DELETE_SUCCESS",
                    "User",
                    id.ToString(),
                    currentUser,
                    "User deleted successfully",
                    Domain.Entities.LogLevel.Success
                );
            }
            else
            {
                await _logService.LogActionAsync(
                    "DELETE_FAILED",
                    "User",
                    id.ToString(),
                    currentUser,
                    result.Message,
                    Domain.Entities.LogLevel.Error
                );
            }

            return ProcessServiceResponse(result);
        }
        catch (Exception ex)
        {
            await _logService.LogActionAsync(
                "DELETE_ERROR",
                "User",
                id.ToString(),
                GetCurrentUser(),
                $"Exception: {ex.Message}",
                Domain.Entities.LogLevel.Error
            );
            throw;
        }
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
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();

                await _logService.LogActionAsync(
                    "CHANGE_PASSWORD_FAILED",
                    "User",
                    id.ToString(),
                    GetCurrentUser(),
                    $"Validation errors: {string.Join(", ", errors)}",
                    Domain.Entities.LogLevel.Warning
                );

                return ValidationErrorResponse<bool>(ModelState);
            }

            var currentUser = GetCurrentUser();
            var result = await _userService.ChangePasswordAsync(id, dto, currentUser);

            if (result.Success)
            {
                await _logService.LogActionAsync(
                    "CHANGE_PASSWORD_SUCCESS",
                    "User",
                    id.ToString(),
                    currentUser,
                    "Password changed successfully",
                    Domain.Entities.LogLevel.Success
                );
            }
            else
            {
                await _logService.LogActionAsync(
                    "CHANGE_PASSWORD_FAILED",
                    "User",
                    id.ToString(),
                    currentUser,
                    result.Message,
                    Domain.Entities.LogLevel.Warning
                );
            }

            return ProcessServiceResponse(result);
        }
        catch (Exception ex)
        {
            await _logService.LogActionAsync(
                "CHANGE_PASSWORD_ERROR",
                "User",
                id.ToString(),
                GetCurrentUser(),
                $"Exception: {ex.Message}",
                Domain.Entities.LogLevel.Error
            );
            throw;
        }
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
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();

                await _logService.LogActionAsync(
                    "ADMIN_CHANGE_PASSWORD_FAILED",
                    "User",
                    id.ToString(),
                    GetCurrentUser(),
                    $"Validation errors: {string.Join(", ", errors)}",
                    Domain.Entities.LogLevel.Warning
                );

                return ValidationErrorResponse<bool>(ModelState);
            }

            var currentUser = GetCurrentUser();
            var result = await _userService.AdminChangePasswordAsync(id, dto, currentUser);

            if (result.Success)
            {
                await _logService.LogActionAsync(
                    "ADMIN_CHANGE_PASSWORD_SUCCESS",
                    "User",
                    id.ToString(),
                    currentUser,
                    "Password changed successfully by admin",
                    Domain.Entities.LogLevel.Success
                );
            }
            else
            {
                await _logService.LogActionAsync(
                    "ADMIN_CHANGE_PASSWORD_FAILED",
                    "User",
                    id.ToString(),
                    currentUser,
                    result.Message,
                    Domain.Entities.LogLevel.Warning
                );
            }

            return ProcessServiceResponse(result);
        }
        catch (Exception ex)
        {
            await _logService.LogActionAsync(
                "ADMIN_CHANGE_PASSWORD_ERROR",
                "User",
                id.ToString(),
                GetCurrentUser(),
                $"Exception: {ex.Message}",
                Domain.Entities.LogLevel.Error
            );
            throw;
        }
    }
}

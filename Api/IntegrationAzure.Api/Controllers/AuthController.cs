using Microsoft.AspNetCore.Mvc;
using IntegrationAzure.Api.Application.DTOs;
using IntegrationAzure.Api.Application.Services;

namespace IntegrationAzure.Api.Controllers;

/// <summary>
/// Controller para operações de autenticação
/// Gerencia login, logout e validação de sessão
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : BaseController
{
    private readonly UserService _userService;

    public AuthController(UserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    /// <summary>
    /// Realiza o login do usuário
    /// </summary>
    /// <param name="dto">Dados de login</param>
    /// <returns>Dados do usuário autenticado</returns>
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponseDto<UserDto>>> Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationErrorResponse<UserDto>(ModelState);
        }

        var result = await _userService.AuthenticateAsync(dto.Email, dto.Password);
        return ProcessServiceResponse(result);
    }

    /// <summary>
    /// Valida se o usuário está autenticado
    /// </summary>
    /// <returns>Dados do usuário atual</returns>
    [HttpGet("validate")]
    public ActionResult<ApiResponseDto<UserDto>> Validate()
    {
        // Por enquanto, retorna sucesso para qualquer chamada
        // TODO: Implementar validação de token JWT
        return Ok(new ApiResponseDto<UserDto>
        {
            Success = true,
            Message = "Token válido",
            Data = null
        });
    }

    /// <summary>
    /// Realiza o logout do usuário
    /// </summary>
    /// <returns>Resultado da operação</returns>
    [HttpPost("logout")]
    public ActionResult<ApiResponseDto<bool>> Logout()
    {
        // Por enquanto, apenas retorna sucesso
        // TODO: Implementar invalidação de token JWT
        return Ok(new ApiResponseDto<bool>
        {
            Success = true,
            Message = "Logout realizado com sucesso",
            Data = true
        });
    }
}

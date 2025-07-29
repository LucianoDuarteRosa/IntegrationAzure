using Microsoft.AspNetCore.Mvc;
using IntegrationAzure.Api.Application.DTOs;
using IntegrationAzure.Api.Application.Services;

namespace IntegrationAzure.Api.Controllers;

/// <summary>
/// Controller para operações CRUD de perfis
/// Gerencia os tipos de perfil: Desenvolvedor, Administrador e Usuário
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProfilesController : BaseController
{
    private readonly ProfileService _profileService;

    public ProfilesController(ProfileService profileService)
    {
        _profileService = profileService ?? throw new ArgumentNullException(nameof(profileService));
    }

    /// <summary>
    /// Obtém todos os perfis
    /// </summary>
    /// <returns>Lista de perfis</returns>
    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<ProfileDto>>>> GetAll()
    {
        var result = await _profileService.GetAllAsync();
        return ProcessServiceResponse(result);
    }

    /// <summary>
    /// Obtém perfis ativos para dropdown/select
    /// </summary>
    /// <returns>Lista simplificada de perfis ativos</returns>
    [HttpGet("active")]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<SimpleProfileDto>>>> GetActiveProfiles()
    {
        var result = await _profileService.GetActiveProfilesAsync();
        return ProcessServiceResponse(result);
    }

    /// <summary>
    /// Obtém um perfil por ID
    /// </summary>
    /// <param name="id">ID do perfil</param>
    /// <returns>Perfil encontrado</returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponseDto<ProfileDto>>> GetById(Guid id)
    {
        var result = await _profileService.GetByIdAsync(id);
        return ProcessServiceResponse(result);
    }

    /// <summary>
    /// Cria um novo perfil
    /// </summary>
    /// <param name="dto">Dados do perfil a ser criado</param>
    /// <returns>Perfil criado</returns>
    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<ProfileDto>>> Create([FromBody] CreateProfileDto dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationErrorResponse<ProfileDto>(ModelState);
        }

        var currentUser = GetCurrentUser();
        var result = await _profileService.CreateAsync(dto, currentUser);

        if (result.Success)
        {
            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);
        }

        return ProcessServiceResponse(result);
    }

    /// <summary>
    /// Atualiza um perfil existente
    /// </summary>
    /// <param name="id">ID do perfil</param>
    /// <param name="dto">Dados atualizados do perfil</param>
    /// <returns>Perfil atualizado</returns>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponseDto<ProfileDto>>> Update(Guid id, [FromBody] UpdateProfileDto dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationErrorResponse<ProfileDto>(ModelState);
        }

        var currentUser = GetCurrentUser();
        var result = await _profileService.UpdateAsync(id, dto, currentUser);
        return ProcessServiceResponse(result);
    }

    /// <summary>
    /// Remove um perfil (soft delete)
    /// </summary>
    /// <param name="id">ID do perfil</param>
    /// <returns>Resultado da operação</returns>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponseDto<bool>>> Delete(Guid id)
    {
        var currentUser = GetCurrentUser();
        var result = await _profileService.DeleteAsync(id, currentUser);
        return ProcessServiceResponse(result);
    }
}

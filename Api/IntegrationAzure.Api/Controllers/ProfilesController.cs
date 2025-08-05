using Microsoft.AspNetCore.Mvc;
using IntegrationAzure.Api.Application.DTOs;
using IntegrationAzure.Api.Application.Services;

namespace IntegrationAzure.Api.Controllers;

/// <summary>
/// Controller para operações com perfis
/// Gerencia apenas consulta de perfis ativos para uso em dropdowns
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
    /// Obtém perfis ativos para dropdown/select
    /// </summary>
    /// <returns>Lista simplificada de perfis ativos</returns>
    [HttpGet("active")]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<SimpleProfileDto>>>> GetActiveProfiles()
    {
        var result = await _profileService.GetActiveProfilesAsync();
        return ProcessServiceResponse(result);
    }
}

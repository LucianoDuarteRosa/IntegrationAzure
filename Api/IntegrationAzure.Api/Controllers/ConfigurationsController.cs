using IntegrationAzure.Api.Application.DTOs;
using IntegrationAzure.Api.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationAzure.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfigurationsController : BaseController
    {
        private readonly ConfigurationService _configurationService;
        private readonly LogService _logService;

        public ConfigurationsController(ConfigurationService configurationService, LogService logService)
        {
            _configurationService = configurationService;
            _logService = logService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<ConfigurationDto>>>> GetAll()
        {
            try
            {
                var configurations = await _configurationService.GetAllAsync();
                return SuccessResponse(configurations ?? new List<ConfigurationDto>(), "Configurações recuperadas com sucesso");
            }
            catch (Exception ex)
            {
                await _logService.LogActionAsync(
                    "GET_ALL_ERROR",
                    "Configuration",
                    null,
                    GetCurrentUser(),
                    $"Exception: {ex.Message}",
                    Domain.Entities.LogLevel.Error
                );

                return ErrorResponse<IEnumerable<ConfigurationDto>>($"Erro ao buscar configurações: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<ConfigurationDto>>> Create([FromBody] CreateConfigurationDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();

                    await _logService.LogActionAsync(
                        "CREATE_FAILED",
                        "Configuration",
                        null,
                        GetCurrentUser(),
                        $"Validation errors: {string.Join(", ", errors)}",
                        Domain.Entities.LogLevel.Warning
                    );

                    return ValidationErrorResponse<ConfigurationDto>(ModelState);
                }

                var currentUser = GetCurrentUser();
                var configuration = await _configurationService.CreateAsync(createDto, currentUser);

                await _logService.LogActionAsync(
                    "CREATE_SUCCESS",
                    "Configuration",
                    configuration.Id.ToString(),
                    currentUser,
                    $"Created configuration: {configuration.Key}",
                    Domain.Entities.LogLevel.Success
                );

                return SuccessResponse(configuration, "Configuração criada com sucesso");
            }
            catch (InvalidOperationException ex)
            {
                await _logService.LogActionAsync(
                    "CREATE_FAILED",
                    "Configuration",
                    null,
                    GetCurrentUser(),
                    $"Invalid operation: {ex.Message}",
                    Domain.Entities.LogLevel.Warning
                );

                return ErrorResponse<ConfigurationDto>(ex.Message, null, 409);
            }
            catch (Exception ex)
            {
                await _logService.LogActionAsync(
                    "CREATE_ERROR",
                    "Configuration",
                    null,
                    GetCurrentUser(),
                    $"Exception: {ex.Message}",
                    Domain.Entities.LogLevel.Error
                );

                return ErrorResponse<ConfigurationDto>($"Erro ao criar configuração: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponseDto<ConfigurationDto>>> Update(int id, [FromBody] UpdateConfigurationDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();

                    await _logService.LogActionAsync(
                        "UPDATE_FAILED",
                        "Configuration",
                        id.ToString(),
                        GetCurrentUser(),
                        $"Validation errors: {string.Join(", ", errors)}",
                        Domain.Entities.LogLevel.Warning
                    );

                    return ValidationErrorResponse<ConfigurationDto>(ModelState);
                }

                var currentUser = GetCurrentUser();
                var configuration = await _configurationService.UpdateAsync(id, updateDto, currentUser);

                await _logService.LogActionAsync(
                    "UPDATE_SUCCESS",
                    "Configuration",
                    id.ToString(),
                    currentUser,
                    $"Updated configuration: {configuration.Key}",
                    Domain.Entities.LogLevel.Success
                );

                return SuccessResponse(configuration, "Configuração atualizada com sucesso");
            }
            catch (InvalidOperationException ex)
            {
                await _logService.LogActionAsync(
                    "UPDATE_FAILED",
                    "Configuration",
                    id.ToString(),
                    GetCurrentUser(),
                    $"Invalid operation: {ex.Message}",
                    Domain.Entities.LogLevel.Warning
                );

                return ErrorResponse<ConfigurationDto>(ex.Message, null, 404);
            }
            catch (Exception ex)
            {
                await _logService.LogActionAsync(
                    "UPDATE_ERROR",
                    "Configuration",
                    id.ToString(),
                    GetCurrentUser(),
                    $"Exception: {ex.Message}",
                    Domain.Entities.LogLevel.Error
                );

                return ErrorResponse<ConfigurationDto>($"Erro ao atualizar configuração: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponseDto<bool>>> Delete(int id)
        {
            try
            {
                var currentUser = GetCurrentUser();
                await _configurationService.DeleteAsync(id);

                await _logService.LogActionAsync(
                    "DELETE_SUCCESS",
                    "Configuration",
                    id.ToString(),
                    currentUser,
                    "Configuration deleted successfully",
                    Domain.Entities.LogLevel.Success
                );

                return SuccessResponse(true, "Configuração excluída com sucesso");
            }
            catch (InvalidOperationException ex)
            {
                await _logService.LogActionAsync(
                    "DELETE_FAILED",
                    "Configuration",
                    id.ToString(),
                    GetCurrentUser(),
                    $"Invalid operation: {ex.Message}",
                    Domain.Entities.LogLevel.Warning
                );

                return ErrorResponse<bool>(ex.Message, null, 404);
            }
            catch (Exception ex)
            {
                await _logService.LogActionAsync(
                    "DELETE_ERROR",
                    "Configuration",
                    id.ToString(),
                    GetCurrentUser(),
                    $"Exception: {ex.Message}",
                    Domain.Entities.LogLevel.Error
                );

                return ErrorResponse<bool>($"Erro ao excluir configuração: {ex.Message}");
            }
        }
    }
}

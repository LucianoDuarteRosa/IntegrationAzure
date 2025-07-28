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

        public ConfigurationsController(ConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<ConfigurationDto>>>> GetAll()
        {
            try
            {
                var configurations = await _configurationService.GetAllAsync();
                return SuccessResponse(configurations, "Configurações recuperadas com sucesso");
            }
            catch (Exception ex)
            {
                return ErrorResponse<IEnumerable<ConfigurationDto>>($"Erro ao buscar configurações: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDto<ConfigurationDto>>> GetById(int id)
        {
            try
            {
                var configuration = await _configurationService.GetByIdAsync(id);
                if (configuration == null)
                {
                    return ErrorResponse<ConfigurationDto>("Configuração não encontrada", null, 404);
                }

                return SuccessResponse(configuration, "Configuração recuperada com sucesso");
            }
            catch (Exception ex)
            {
                return ErrorResponse<ConfigurationDto>($"Erro ao buscar configuração: {ex.Message}");
            }
        }

        [HttpGet("key/{key}")]
        public async Task<ActionResult<ApiResponseDto<ConfigurationDto>>> GetByKey(string key)
        {
            try
            {
                var configuration = await _configurationService.GetByKeyAsync(key);
                if (configuration == null)
                {
                    return ErrorResponse<ConfigurationDto>("Configuração não encontrada", null, 404);
                }

                return SuccessResponse(configuration, "Configuração recuperada com sucesso");
            }
            catch (Exception ex)
            {
                return ErrorResponse<ConfigurationDto>($"Erro ao buscar configuração: {ex.Message}");
            }
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<ConfigurationDto>>>> GetByCategory(string category)
        {
            try
            {
                var configurations = await _configurationService.GetByCategoryAsync(category);
                return SuccessResponse(configurations, "Configurações da categoria recuperadas com sucesso");
            }
            catch (Exception ex)
            {
                return ErrorResponse<IEnumerable<ConfigurationDto>>($"Erro ao buscar configurações por categoria: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<ConfigurationDto>>> Create([FromBody] CreateConfigurationDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return ValidationErrorResponse<ConfigurationDto>(ModelState);
                }

                var configuration = await _configurationService.CreateAsync(createDto, GetCurrentUser());
                return CreatedAtAction(nameof(GetById), new { id = configuration.Id },
                    SuccessResponse(configuration, "Configuração criada com sucesso").Value);
            }
            catch (InvalidOperationException ex)
            {
                return ErrorResponse<ConfigurationDto>(ex.Message, null, 409);
            }
            catch (Exception ex)
            {
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
                    return ValidationErrorResponse<ConfigurationDto>(ModelState);
                }

                var configuration = await _configurationService.UpdateAsync(id, updateDto, GetCurrentUser());
                return SuccessResponse(configuration, "Configuração atualizada com sucesso");
            }
            catch (InvalidOperationException ex)
            {
                return ErrorResponse<ConfigurationDto>(ex.Message, null, 404);
            }
            catch (Exception ex)
            {
                return ErrorResponse<ConfigurationDto>($"Erro ao atualizar configuração: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponseDto<object>>> Delete(int id)
        {
            try
            {
                await _configurationService.DeleteAsync(id);
                return SuccessResponse<object>(null, "Configuração excluída com sucesso");
            }
            catch (InvalidOperationException ex)
            {
                return ErrorResponse<object>(ex.Message, null, 404);
            }
            catch (Exception ex)
            {
                return ErrorResponse<object>($"Erro ao excluir configuração: {ex.Message}");
            }
        }
    }
}

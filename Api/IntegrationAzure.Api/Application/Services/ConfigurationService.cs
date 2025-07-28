using IntegrationAzure.Api.Application.DTOs;
using IntegrationAzure.Api.Domain.Entities;
using IntegrationAzure.Api.Domain.Interfaces;

namespace IntegrationAzure.Api.Application.Services
{
    public class ConfigurationService
    {
        private readonly IConfigurationRepository _configurationRepository;

        public ConfigurationService(IConfigurationRepository configurationRepository)
        {
            _configurationRepository = configurationRepository;
        }

        public async Task<IEnumerable<ConfigurationDto>> GetAllAsync()
        {
            var configurations = await _configurationRepository.GetAllAsync();
            return configurations.Select(MapToDto);
        }

        public async Task<ConfigurationDto?> GetByIdAsync(int id)
        {
            var configuration = await _configurationRepository.GetByIdAsync(id);
            return configuration == null ? null : MapToDto(configuration);
        }

        public async Task<ConfigurationDto?> GetByKeyAsync(string key)
        {
            var configuration = await _configurationRepository.GetByKeyAsync(key);
            return configuration == null ? null : MapToDto(configuration);
        }

        public async Task<IEnumerable<ConfigurationDto>> GetByCategoryAsync(string category)
        {
            var configurations = await _configurationRepository.GetByCategoryAsync(category);
            return configurations.Select(MapToDto);
        }

        public async Task<ConfigurationDto> CreateAsync(CreateConfigurationDto createDto, string createdBy)
        {
            // Verificar se já existe uma configuração com a mesma chave
            if (await _configurationRepository.ExistsAsync(createDto.Key))
            {
                throw new InvalidOperationException($"Já existe uma configuração com a chave '{createDto.Key}'");
            }

            var configuration = new Configuration
            {
                Key = createDto.Key,
                Value = createDto.Value,
                Description = createDto.Description,
                Category = createDto.Category,
                IsSecret = createDto.IsSecret,
                IsActive = createDto.IsActive,
                CreatedBy = createdBy,
                UpdatedBy = createdBy
            };

            var createdConfiguration = await _configurationRepository.CreateAsync(configuration);
            return MapToDto(createdConfiguration);
        }

        public async Task<ConfigurationDto> UpdateAsync(int id, UpdateConfigurationDto updateDto, string updatedBy)
        {
            var existingConfiguration = await _configurationRepository.GetByIdAsync(id);
            if (existingConfiguration == null)
            {
                throw new InvalidOperationException("Configuração não encontrada");
            }

            existingConfiguration.Value = updateDto.Value;
            existingConfiguration.Description = updateDto.Description;
            existingConfiguration.Category = updateDto.Category;
            existingConfiguration.IsSecret = updateDto.IsSecret;
            existingConfiguration.IsActive = updateDto.IsActive;
            existingConfiguration.UpdatedBy = updatedBy;

            var updatedConfiguration = await _configurationRepository.UpdateAsync(existingConfiguration);
            return MapToDto(updatedConfiguration);
        }

        public async Task DeleteAsync(int id)
        {
            var configuration = await _configurationRepository.GetByIdAsync(id);
            if (configuration == null)
            {
                throw new InvalidOperationException("Configuração não encontrada");
            }

            await _configurationRepository.DeleteAsync(id);
        }

        private static ConfigurationDto MapToDto(Configuration configuration)
        {
            return new ConfigurationDto
            {
                Id = configuration.Id,
                Key = configuration.Key,
                Value = configuration.IsSecret ? "*****" : configuration.Value, // Mascarar valores secretos
                Description = configuration.Description,
                Category = configuration.Category,
                IsSecret = configuration.IsSecret,
                IsActive = configuration.IsActive,
                CreatedAt = configuration.CreatedAt,
                UpdatedAt = configuration.UpdatedAt,
                CreatedBy = configuration.CreatedBy,
                UpdatedBy = configuration.UpdatedBy
            };
        }

        // Método especial para obter valor sem mascarar (para uso interno)
        public async Task<string?> GetValueAsync(string key)
        {
            var configuration = await _configurationRepository.GetByKeyAsync(key);
            return configuration?.Value;
        }
    }
}

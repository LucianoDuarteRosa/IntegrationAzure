using IntegrationAzure.Api.Application.DTOs;
using IntegrationAzure.Api.Domain.Entities;
using IntegrationAzure.Api.Domain.Interfaces;

namespace IntegrationAzure.Api.Application.Services;

/// <summary>
/// Serviço para operações relacionadas a perfis
/// Implementa regras de negócio e orquestração entre domínio e infraestrutura
/// </summary>
public class ProfileService
{
    private readonly IProfileRepository _profileRepository;

    public ProfileService(IProfileRepository profileRepository)
    {
        _profileRepository = profileRepository ?? throw new ArgumentNullException(nameof(profileRepository));
    }

    /// <summary>
    /// Cria um novo perfil
    /// </summary>
    public async Task<ApiResponseDto<ProfileDto>> CreateAsync(CreateProfileDto dto, string currentUser)
    {
        try
        {
            // Verificar se já existe um perfil com o mesmo nome
            if (await _profileRepository.ExistsByNameAsync(dto.Name))
            {
                return new ApiResponseDto<ProfileDto>
                {
                    Success = false,
                    Message = "Já existe um perfil com este nome"
                };
            }

            var profile = new Profile
            {
                Name = dto.Name,
                Description = dto.Description,
                CreatedBy = currentUser
            };

            var createdProfile = await _profileRepository.AddAsync(profile);
            await _profileRepository.SaveChangesAsync();

            var responseDto = MapToProfileDto(createdProfile);
            return new ApiResponseDto<ProfileDto>
            {
                Success = true,
                Message = "Perfil criado com sucesso",
                Data = responseDto
            };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<ProfileDto>
            {
                Success = false,
                Message = $"Erro interno: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Obtém todos os perfis
    /// </summary>
    public async Task<ApiResponseDto<IEnumerable<ProfileDto>>> GetAllAsync()
    {
        try
        {
            var profiles = await _profileRepository.GetAllAsync();
            var responseDtos = profiles.Select(MapToProfileDto);
            return new ApiResponseDto<IEnumerable<ProfileDto>>
            {
                Success = true,
                Data = responseDtos
            };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<IEnumerable<ProfileDto>>
            {
                Success = false,
                Message = $"Erro interno: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Obtém um perfil por ID
    /// </summary>
    public async Task<ApiResponseDto<ProfileDto>> GetByIdAsync(Guid id)
    {
        try
        {
            var profile = await _profileRepository.GetByIdAsync(id);
            if (profile == null)
            {
                return new ApiResponseDto<ProfileDto>
                {
                    Success = false,
                    Message = "Perfil não encontrado"
                };
            }

            var responseDto = MapToProfileDto(profile);
            return new ApiResponseDto<ProfileDto>
            {
                Success = true,
                Data = responseDto
            };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<ProfileDto>
            {
                Success = false,
                Message = $"Erro interno: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Atualiza um perfil
    /// </summary>
    public async Task<ApiResponseDto<ProfileDto>> UpdateAsync(Guid id, UpdateProfileDto dto, string currentUser)
    {
        try
        {
            var profile = await _profileRepository.GetByIdAsync(id);
            if (profile == null)
            {
                return new ApiResponseDto<ProfileDto>
                {
                    Success = false,
                    Message = "Perfil não encontrado"
                };
            }

            // Verificar se já existe outro perfil com o mesmo nome
            var existingProfile = await _profileRepository.GetByNameAsync(dto.Name);
            if (existingProfile != null && existingProfile.Id != id)
            {
                return new ApiResponseDto<ProfileDto>
                {
                    Success = false,
                    Message = "Já existe um perfil com este nome"
                };
            }

            profile.Name = dto.Name;
            profile.Description = dto.Description;
            profile.UpdatedAt = DateTime.UtcNow;
            profile.UpdatedBy = currentUser;

            var updatedProfile = await _profileRepository.UpdateAsync(profile);
            await _profileRepository.SaveChangesAsync();

            var responseDto = MapToProfileDto(updatedProfile);
            return new ApiResponseDto<ProfileDto>
            {
                Success = true,
                Message = "Perfil atualizado com sucesso",
                Data = responseDto
            };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<ProfileDto>
            {
                Success = false,
                Message = $"Erro interno: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Remove um perfil (soft delete)
    /// </summary>
    public async Task<ApiResponseDto<bool>> DeleteAsync(Guid id, string currentUser)
    {
        try
        {
            var profile = await _profileRepository.GetByIdAsync(id);
            if (profile == null)
            {
                return new ApiResponseDto<bool>
                {
                    Success = false,
                    Message = "Perfil não encontrado"
                };
            }

            // Verificar se existem usuários associados a este perfil
            if (profile.Users.Any(u => u.IsActive))
            {
                return new ApiResponseDto<bool>
                {
                    Success = false,
                    Message = "Não é possível excluir um perfil que possui usuários associados"
                };
            }

            profile.IsActive = false;
            profile.UpdatedAt = DateTime.UtcNow;
            profile.UpdatedBy = currentUser;

            await _profileRepository.UpdateAsync(profile);
            await _profileRepository.SaveChangesAsync();

            return new ApiResponseDto<bool>
            {
                Success = true,
                Message = "Perfil removido com sucesso",
                Data = true
            };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<bool>
            {
                Success = false,
                Message = $"Erro interno: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Obtém perfis ativos para dropdown/select
    /// </summary>
    public async Task<ApiResponseDto<IEnumerable<SimpleProfileDto>>> GetActiveProfilesAsync()
    {
        try
        {
            var profiles = await _profileRepository.GetActiveProfilesAsync();
            var responseDtos = profiles.Select(p => new SimpleProfileDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description
            });
            return new ApiResponseDto<IEnumerable<SimpleProfileDto>>
            {
                Success = true,
                Data = responseDtos
            };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<IEnumerable<SimpleProfileDto>>
            {
                Success = false,
                Message = $"Erro interno: {ex.Message}"
            };
        }
    }

    private static ProfileDto MapToProfileDto(Profile profile)
    {
        return new ProfileDto
        {
            Id = profile.Id,
            Name = profile.Name,
            Description = profile.Description,
            CreatedAt = profile.CreatedAt,
            UpdatedAt = profile.UpdatedAt,
            CreatedBy = profile.CreatedBy,
            UpdatedBy = profile.UpdatedBy,
            IsActive = profile.IsActive,
            UsersCount = profile.Users.Count(u => u.IsActive)
        };
    }
}

using IntegrationAzure.Api.Application.DTOs;
using IntegrationAzure.Api.Domain.Entities;
using IntegrationAzure.Api.Domain.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace IntegrationAzure.Api.Application.Services;

/// <summary>
/// Serviço para operações relacionadas a usuários
/// Implementa regras de negócio e orquestração entre domínio e infraestrutura
/// </summary>
public class UserService
{
    private readonly IUserRepository _userRepository;
    private readonly IProfileRepository _profileRepository;

    public UserService(IUserRepository userRepository, IProfileRepository profileRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _profileRepository = profileRepository ?? throw new ArgumentNullException(nameof(profileRepository));
    }

    /// <summary>
    /// Cria um novo usuário
    /// </summary>
    public async Task<ApiResponseDto<UserDto>> CreateAsync(CreateUserDto dto, string currentUser)
    {
        try
        {
            // Validar se o perfil existe
            var profile = await _profileRepository.GetByIdAsync(dto.ProfileId);
            if (profile == null)
            {
                return new ApiResponseDto<UserDto>
                {
                    Success = false,
                    Message = "Perfil não encontrado"
                };
            }

            // Verificar se já existe um usuário com o mesmo email
            if (await _userRepository.ExistsByEmailAsync(dto.Email))
            {
                return new ApiResponseDto<UserDto>
                {
                    Success = false,
                    Message = "Já existe um usuário com este email"
                };
            }

            // Verificar se já existe um usuário com o mesmo nickname
            if (await _userRepository.ExistsByNicknameAsync(dto.Nickname))
            {
                return new ApiResponseDto<UserDto>
                {
                    Success = false,
                    Message = "Já existe um usuário com este nickname"
                };
            }

            var user = new User
            {
                Name = dto.Name,
                Nickname = dto.Nickname,
                Email = dto.Email,
                Password = HashPassword(dto.Password),
                ProfileImagePath = dto.ProfileImagePath,
                ProfileId = dto.ProfileId,
                CreatedBy = currentUser
            };

            var createdUser = await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            // Buscar o usuário com o perfil para retornar
            var userWithProfile = await _userRepository.GetWithProfileAsync(createdUser.Id);
            var responseDto = MapToUserDto(userWithProfile!);
            return new ApiResponseDto<UserDto>
            {
                Success = true,
                Message = "Usuário criado com sucesso",
                Data = responseDto
            };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<UserDto>
            {
                Success = false,
                Message = $"Erro interno: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Obtém todos os usuários
    /// </summary>
    public async Task<ApiResponseDto<IEnumerable<UserDto>>> GetAllAsync()
    {
        try
        {
            var users = await _userRepository.GetAllWithProfilesAsync();
            var responseDtos = users.Select(MapToUserDto);
            return new ApiResponseDto<IEnumerable<UserDto>>
            {
                Success = true,
                Data = responseDtos
            };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<IEnumerable<UserDto>>
            {
                Success = false,
                Message = $"Erro interno: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Obtém um usuário por ID
    /// </summary>
    public async Task<ApiResponseDto<UserDto>> GetByIdAsync(Guid id)
    {
        try
        {
            var user = await _userRepository.GetWithProfileAsync(id);
            if (user == null)
            {
                return new ApiResponseDto<UserDto>
                {
                    Success = false,
                    Message = "Usuário não encontrado"
                };
            }

            var responseDto = MapToUserDto(user);
            return new ApiResponseDto<UserDto>
            {
                Success = true,
                Data = responseDto
            };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<UserDto>
            {
                Success = false,
                Message = $"Erro interno: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Atualiza um usuário
    /// </summary>
    public async Task<ApiResponseDto<UserDto>> UpdateAsync(Guid id, UpdateUserDto dto, string currentUser)
    {
        try
        {
            var user = await _userRepository.GetWithProfileAsync(id);
            if (user == null)
            {
                return new ApiResponseDto<UserDto>
                {
                    Success = false,
                    Message = "Usuário não encontrado"
                };
            }

            // Validar se o perfil existe
            var profile = await _profileRepository.GetByIdAsync(dto.ProfileId);
            if (profile == null)
            {
                return new ApiResponseDto<UserDto>
                {
                    Success = false,
                    Message = "Perfil não encontrado"
                };
            }

            // Verificar se já existe outro usuário com o mesmo email
            var existingUserByEmail = await _userRepository.GetByEmailAsync(dto.Email);
            if (existingUserByEmail != null && existingUserByEmail.Id != id)
            {
                return new ApiResponseDto<UserDto>
                {
                    Success = false,
                    Message = "Já existe um usuário com este email"
                };
            }

            // Verificar se já existe outro usuário com o mesmo nickname
            var existingUserByNickname = await _userRepository.GetByNicknameAsync(dto.Nickname);
            if (existingUserByNickname != null && existingUserByNickname.Id != id)
            {
                return new ApiResponseDto<UserDto>
                {
                    Success = false,
                    Message = "Já existe um usuário com este nickname"
                };
            }

            user.Name = dto.Name;
            user.Nickname = dto.Nickname;
            user.Email = dto.Email;
            user.ProfileImagePath = dto.ProfileImagePath;
            user.ProfileId = dto.ProfileId;
            user.UpdatedAt = DateTime.UtcNow;
            user.UpdatedBy = currentUser;

            var updatedUser = await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            // Buscar o usuário atualizado com o perfil
            var userWithProfile = await _userRepository.GetWithProfileAsync(updatedUser.Id);
            var responseDto = MapToUserDto(userWithProfile!);
            return new ApiResponseDto<UserDto>
            {
                Success = true,
                Message = "Usuário atualizado com sucesso",
                Data = responseDto
            };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<UserDto>
            {
                Success = false,
                Message = $"Erro interno: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Remove um usuário (soft delete)
    /// </summary>
    public async Task<ApiResponseDto<bool>> DeleteAsync(Guid id, string currentUser)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return new ApiResponseDto<bool>
                {
                    Success = false,
                    Message = "Usuário não encontrado"
                };
            }

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;
            user.UpdatedBy = currentUser;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return new ApiResponseDto<bool>
            {
                Success = true,
                Message = "Usuário removido com sucesso",
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
    /// Altera a senha de um usuário
    /// </summary>
    public async Task<ApiResponseDto<bool>> ChangePasswordAsync(Guid id, ChangePasswordDto dto, string currentUser)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return new ApiResponseDto<bool>
                {
                    Success = false,
                    Message = "Usuário não encontrado"
                };
            }

            // Verificar se a senha atual está correta
            if (!VerifyPassword(dto.CurrentPassword, user.Password))
            {
                return new ApiResponseDto<bool>
                {
                    Success = false,
                    Message = "Senha atual incorreta"
                };
            }

            user.Password = HashPassword(dto.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;
            user.UpdatedBy = currentUser;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return new ApiResponseDto<bool>
            {
                Success = true,
                Message = "Senha alterada com sucesso",
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
    /// Obtém usuários por perfil
    /// </summary>
    public async Task<ApiResponseDto<IEnumerable<SimpleUserDto>>> GetByProfileAsync(Guid profileId)
    {
        try
        {
            var users = await _userRepository.GetByProfileIdAsync(profileId);
            var responseDtos = users.Select(u => new SimpleUserDto
            {
                Id = u.Id,
                Name = u.Name,
                Nickname = u.Nickname,
                Email = u.Email,
                ProfileImagePath = u.ProfileImagePath,
                ProfileName = u.Profile.Name
            });
            return new ApiResponseDto<IEnumerable<SimpleUserDto>>
            {
                Success = true,
                Data = responseDtos
            };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<IEnumerable<SimpleUserDto>>
            {
                Success = false,
                Message = $"Erro interno: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Gera hash da senha usando SHA256
    /// </summary>
    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    /// <summary>
    /// Verifica se a senha fornecida corresponde ao hash armazenado
    /// </summary>
    private static bool VerifyPassword(string password, string hashedPassword)
    {
        var hashedInput = HashPassword(password);
        return hashedInput == hashedPassword;
    }

    private static UserDto MapToUserDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Nickname = user.Nickname,
            Email = user.Email,
            ProfileImagePath = user.ProfileImagePath,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            CreatedBy = user.CreatedBy,
            UpdatedBy = user.UpdatedBy,
            IsActive = user.IsActive,
            Profile = new SimpleProfileDto
            {
                Id = user.Profile.Id,
                Name = user.Profile.Name,
                Description = user.Profile.Description
            }
        };
    }
}

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
    private readonly IWebHostEnvironment _environment;

    public UserService(IUserRepository userRepository, IProfileRepository profileRepository, IWebHostEnvironment environment)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _profileRepository = profileRepository ?? throw new ArgumentNullException(nameof(profileRepository));
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));
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

            // Gerenciar substituição de imagem de perfil
            var oldImagePath = user.ProfileImagePath;
            var newImagePath = dto.ProfileImagePath;

            // Se a imagem mudou, deletar a antiga
            if (!string.IsNullOrEmpty(oldImagePath) &&
                !string.IsNullOrEmpty(newImagePath) &&
                oldImagePath != newImagePath)
            {
                DeleteProfileImage(oldImagePath);
            }
            // Se removeu a imagem (novo valor é null ou vazio), deletar a antiga
            else if (!string.IsNullOrEmpty(oldImagePath) && string.IsNullOrEmpty(newImagePath))
            {
                DeleteProfileImage(oldImagePath);
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

            // Deletar imagem de perfil se existir
            if (!string.IsNullOrEmpty(user.ProfileImagePath))
            {
                DeleteProfileImage(user.ProfileImagePath);
                user.ProfileImagePath = null; // Limpar referência no banco
            }

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
    /// Altera a senha de um usuário com base na hierarquia de perfis
    /// </summary>
    public async Task<ApiResponseDto<bool>> ChangePasswordAsync(Guid id, ChangePasswordDto dto, string currentUser)
    {
        try
        {
            var user = await _userRepository.GetWithProfileAsync(id);
            if (user == null)
            {
                return new ApiResponseDto<bool>
                {
                    Success = false,
                    Message = "Usuário não encontrado"
                };
            }

            // Buscar o usuário atual para verificar hierarquia
            var currentUserEntity = await _userRepository.GetByEmailAsync(currentUser);
            if (currentUserEntity == null)
            {
                return new ApiResponseDto<bool>
                {
                    Success = false,
                    Message = "Usuário atual não encontrado"
                };
            }

            var currentUserProfile = await _profileRepository.GetByIdAsync(currentUserEntity.ProfileId);
            var targetUserProfile = user.Profile;

            // Verificar se precisa da senha atual baseado na hierarquia
            bool needsCurrentPassword = ShouldRequireCurrentPassword(currentUserProfile?.Name, targetUserProfile?.Name);

            if (needsCurrentPassword)
            {
                // Verificar se a senha atual está correta
                if (!VerifyPassword(dto.CurrentPassword, user.Password))
                {
                    return new ApiResponseDto<bool>
                    {
                        Success = false,
                        Message = "Senha atual incorreta"
                    };
                }
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
    /// Altera a senha de um usuário por hierarquia (sem necessidade da senha atual)
    /// </summary>
    public async Task<ApiResponseDto<bool>> AdminChangePasswordAsync(Guid id, AdminChangePasswordDto dto, string currentUser)
    {
        try
        {
            var user = await _userRepository.GetWithProfileAsync(id);
            if (user == null)
            {
                return new ApiResponseDto<bool>
                {
                    Success = false,
                    Message = "Usuário não encontrado"
                };
            }

            // Buscar o usuário atual para verificar hierarquia
            var currentUserEntity = await _userRepository.GetByEmailAsync(currentUser);
            if (currentUserEntity == null)
            {
                return new ApiResponseDto<bool>
                {
                    Success = false,
                    Message = "Usuário atual não encontrado"
                };
            }

            var currentUserProfile = await _profileRepository.GetByIdAsync(currentUserEntity.ProfileId);
            var targetUserProfile = user.Profile;

            // Verificar se tem permissão para alterar senha sem senha atual
            if (!CanChangePasswordWithoutCurrent(currentUserProfile?.Name, targetUserProfile?.Name))
            {
                return new ApiResponseDto<bool>
                {
                    Success = false,
                    Message = "Sem permissão para alterar senha deste usuário"
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
    /// Verifica se pode alterar senha sem informar a senha atual
    /// </summary>
    private static bool CanChangePasswordWithoutCurrent(string? currentUserProfile, string? targetUserProfile)
    {
        if (string.IsNullOrEmpty(currentUserProfile) || string.IsNullOrEmpty(targetUserProfile))
            return false;

        // Desenvolvedor pode alterar senha de qualquer um sem precisar da atual
        if (currentUserProfile == "Desenvolvedor")
            return true;

        // Administrador pode alterar senha de Usuários sem precisar da atual
        if (currentUserProfile == "Administrador" && targetUserProfile == "Usuário")
            return true;

        return false;
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
    /// Determina se é necessário informar a senha atual para alterar a senha de um usuário
    /// baseado na hierarquia: Desenvolvedor > Administrador > Usuário
    /// </summary>
    private static bool ShouldRequireCurrentPassword(string? currentUserProfile, string? targetUserProfile)
    {
        if (string.IsNullOrEmpty(currentUserProfile) || string.IsNullOrEmpty(targetUserProfile))
            return true;

        // Desenvolvedor pode alterar senha de qualquer um sem precisar da atual
        if (currentUserProfile == "Desenvolvedor")
            return false;

        // Administrador pode alterar senha de Usuários sem precisar da atual
        if (currentUserProfile == "Administrador" && targetUserProfile == "Usuário")
            return false;

        // Para todos os outros casos, precisa da senha atual
        return true;
    }

    /// <summary>
    /// Verifica se o usuário atual pode visualizar/editar o usuário alvo baseado na hierarquia
    /// </summary>
    private static bool CanAccessUser(string? currentUserProfile, string? targetUserProfile)
    {
        if (string.IsNullOrEmpty(currentUserProfile) || string.IsNullOrEmpty(targetUserProfile))
            return false;

        return currentUserProfile switch
        {
            "Desenvolvedor" => true, // Desenvolvedor acessa todos
            "Administrador" => targetUserProfile != "Desenvolvedor", // Admin não vê Desenvolvedor
            "Usuário" => false, // Usuário não vê outros (apenas próprio perfil)
            _ => false
        };
    }

    /// <summary>
    /// Obtém todos os usuários filtrados baseado no perfil do usuário atual
    /// </summary>
    public async Task<ApiResponseDto<IEnumerable<UserDto>>> GetAllFilteredAsync(string currentUserEmail)
    {
        try
        {
            var currentUser = await _userRepository.GetByEmailAsync(currentUserEmail);
            if (currentUser == null)
            {
                return new ApiResponseDto<IEnumerable<UserDto>>
                {
                    Success = false,
                    Message = "Usuário atual não encontrado"
                };
            }

            var currentUserProfile = await _profileRepository.GetByIdAsync(currentUser.ProfileId);
            var allUsers = await _userRepository.GetAllWithProfilesAsync();

            var filteredUsers = allUsers.Where(user =>
            {
                // Se for o próprio usuário, sempre pode ver
                if (user.Email == currentUserEmail)
                    return true;

                // Aplicar filtro baseado na hierarquia
                return CanAccessUser(currentUserProfile?.Name, user.Profile?.Name);
            });

            var responseDtos = filteredUsers.Select(MapToUserDto);
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
    /// Autentica um usuário com email e senha
    /// </summary>
    public async Task<ApiResponseDto<UserDto>> AuthenticateAsync(string email, string password)
    {
        try
        {
            // Buscar usuário pelo email
            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null)
            {
                return new ApiResponseDto<UserDto>
                {
                    Success = false,
                    Message = "Email ou senha incorretos"
                };
            }

            // Verificar se o usuário está ativo
            if (!user.IsActive)
            {
                return new ApiResponseDto<UserDto>
                {
                    Success = false,
                    Message = "Usuário desativado"
                };
            }

            // Verificar a senha
            if (!VerifyPassword(password, user.Password))
            {
                return new ApiResponseDto<UserDto>
                {
                    Success = false,
                    Message = "Email ou senha incorretos"
                };
            }

            // Retornar dados do usuário autenticado
            var userDto = MapToUserDto(user);
            return new ApiResponseDto<UserDto>
            {
                Success = true,
                Message = "Login realizado com sucesso",
                Data = userDto
            };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<UserDto>
            {
                Success = false,
                Message = "Erro interno no servidor",
                Errors = new List<string> { ex.Message }
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

    /// <summary>
    /// Remove uma imagem de perfil do diretório de uploads
    /// </summary>
    /// <param name="fileName">Nome do arquivo a ser removido</param>
    private void DeleteProfileImage(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return;

        try
        {
            // Garantir que é apenas o nome do arquivo (sem path traversal)
            var cleanFileName = Path.GetFileName(fileName);
            if (cleanFileName != fileName || fileName.Contains(".."))
                return;

            var uploadsDir = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath, "uploads", "users");
            var fullPath = Path.Combine(uploadsDir, cleanFileName);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
        catch (Exception ex)
        {
            // Log do erro mas não falha a operação principal
            Console.WriteLine($"Erro ao deletar imagem de perfil {fileName}: {ex.Message}");
        }
    }

    /// <summary>
    /// Mapeia uma entidade User para UserDto
    /// </summary>
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

using Microsoft.AspNetCore.Mvc;
using IntegrationAzure.Api.Application.DTOs;

namespace IntegrationAzure.Api.Controllers;

/// <summary>
/// Controller para operações de upload de arquivos
/// Gerencia upload de imagens de perfil e outros arquivos do sistema
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class FileUploadController : BaseController
{
    private readonly IWebHostEnvironment _environment;
    private readonly long _maxFileSize = 5 * 1024 * 1024; // 5MB
    private readonly string[] _allowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };

    public FileUploadController(IWebHostEnvironment environment)
    {
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));
    }

    /// <summary>
    /// Upload de imagem de perfil de usuário
    /// </summary>
    /// <param name="file">Arquivo de imagem</param>
    /// <returns>Caminho da imagem salva</returns>
    [HttpPost("user-profile-image")]
    public async Task<ActionResult<ApiResponseDto<string>>> UploadUserProfileImage(IFormFile file)
    {
        try
        {
            // Validações
            if (file == null || file.Length == 0)
            {
                return BadRequest(new ApiResponseDto<string>
                {
                    Success = false,
                    Message = "Nenhum arquivo foi enviado"
                });
            }

            if (file.Length > _maxFileSize)
            {
                return BadRequest(new ApiResponseDto<string>
                {
                    Success = false,
                    Message = $"Arquivo muito grande. Tamanho máximo permitido: {_maxFileSize / (1024 * 1024)}MB"
                });
            }

            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedImageExtensions.Contains(fileExtension))
            {
                return BadRequest(new ApiResponseDto<string>
                {
                    Success = false,
                    Message = $"Tipo de arquivo não permitido. Extensões permitidas: {string.Join(", ", _allowedImageExtensions)}"
                });
            }

            // Criar diretório se não existir
            var uploadsDir = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath, "uploads", "users");
            if (!Directory.Exists(uploadsDir))
            {
                Directory.CreateDirectory(uploadsDir);
            }

            // Gerar nome único para o arquivo
            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadsDir, fileName);

            // Salvar o arquivo
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Retornar apenas o nome do arquivo (sem o caminho completo)
            return Ok(new ApiResponseDto<string>
            {
                Success = true,
                Message = "Imagem enviada com sucesso",
                Data = fileName
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponseDto<string>
            {
                Success = false,
                Message = $"Erro interno do servidor: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// Remove uma imagem de perfil
    /// </summary>
    /// <param name="fileName">Nome do arquivo a ser removido</param>
    /// <returns>Resultado da operação</returns>
    [HttpDelete("user-profile-image")]
    public ActionResult<ApiResponseDto<bool>> DeleteUserProfileImage([FromQuery] string fileName)
    {
        try
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return BadRequest(new ApiResponseDto<bool>
                {
                    Success = false,
                    Message = "Nome do arquivo é obrigatório"
                });
            }

            // Garantir que é apenas o nome do arquivo (sem path traversal)
            var cleanFileName = Path.GetFileName(fileName);
            if (cleanFileName != fileName || fileName.Contains(".."))
            {
                return BadRequest(new ApiResponseDto<bool>
                {
                    Success = false,
                    Message = "Nome de arquivo inválido"
                });
            }

            var uploadsDir = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath, "uploads", "users");
            var fullPath = Path.Combine(uploadsDir, cleanFileName);

            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }

            return Ok(new ApiResponseDto<bool>
            {
                Success = true,
                Message = "Imagem removida com sucesso",
                Data = true
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponseDto<bool>
            {
                Success = false,
                Message = $"Erro interno do servidor: {ex.Message}"
            });
        }
    }
}

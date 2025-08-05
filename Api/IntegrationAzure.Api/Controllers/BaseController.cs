using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using IntegrationAzure.Api.Application.DTOs;

namespace IntegrationAzure.Api.Controllers;

/// <summary>
/// Controller base com métodos utilitários comuns
/// Implementa padrões de resposta consistentes seguindo RESTful
/// 
/// NOTA SOBRE LOGS:
/// - Operações GET não são logadas por padrão para evitar poluição de logs
/// - Apenas erros em operações GET são logados para depuração
/// - Operações de criação, atualização e exclusão são sempre logadas
/// - Logs de sucesso são usados apenas para operações críticas ou administrativas
/// </summary>
[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    /// <summary>
    /// Obtém o usuário atual do contexto HTTP
    /// Por enquanto usa header customizado, mas pode ser integrado com autenticação
    /// NOTA: Implementar autenticação real quando necessário
    /// </summary>
    protected string GetCurrentUser()
    {
        // Tenta obter o usuário do header customizado primeiro
        if (Request.Headers.TryGetValue("X-Current-User", out var userHeader))
        {
            var userEmail = userHeader.FirstOrDefault();
            if (!string.IsNullOrEmpty(userEmail))
            {
                return userEmail;
            }
        }

        // Fallback para autenticação do contexto ou valor padrão do sistema
        return User?.Identity?.Name ?? "system@integrationazure.com";
    }

    /// <summary>
    /// Cria uma resposta de sucesso padronizada
    /// </summary>
    protected ActionResult<ApiResponseDto<T>> SuccessResponse<T>(T data, string message = "Operação realizada com sucesso")
    {
        var response = new ApiResponseDto<T>
        {
            Success = true,
            Message = message,
            Data = data
        };

        return Ok(response);
    }

    /// <summary>
    /// Cria uma resposta de erro padronizada
    /// </summary>
    protected ActionResult<ApiResponseDto<T>> ErrorResponse<T>(string message, List<string>? errors = null, int statusCode = 400)
    {
        // Sanitizar mensagens de erro para não expor detalhes internos
        var sanitizedMessage = message.Contains("Exception") || message.Contains("Stack") || message.Contains("Internal")
            ? "Erro interno do servidor"
            : message;

        var sanitizedErrors = errors?.Select(error =>
            error.Contains("Exception") || error.Contains("Stack") || error.Contains("Internal")
                ? "Erro interno do sistema"
                : error).ToList() ?? new List<string>();

        var response = new ApiResponseDto<T>
        {
            Success = false,
            Message = sanitizedMessage,
            Errors = sanitizedErrors
        };

        return statusCode switch
        {
            400 => BadRequest(response),
            404 => NotFound(response),
            500 => StatusCode(500, response),
            _ => BadRequest(response)
        };
    }

    /// <summary>
    /// Cria uma resposta de erro de validação
    /// </summary>
    protected ActionResult<ApiResponseDto<T>> ValidationErrorResponse<T>(ModelStateDictionary modelState)
    {
        var errors = modelState
            .Where(x => x.Value?.Errors.Count > 0)
            .SelectMany(x => x.Value!.Errors)
            .Select(x => x.ErrorMessage)
            .ToList();

        return ErrorResponse<T>("Dados de entrada inválidos", errors, 400);
    }

    /// <summary>
    /// Processa resposta do serviço e converte para ActionResult
    /// </summary>
    protected ActionResult<ApiResponseDto<T>> ProcessServiceResponse<T>(ApiResponseDto<T> serviceResponse)
    {
        if (serviceResponse.Success)
        {
            return Ok(serviceResponse);
        }

        // Determinar status code baseado na mensagem
        var statusCode = serviceResponse.Message.ToLowerInvariant() switch
        {
            var msg when msg.Contains("não encontrad") => 404,
            var msg when msg.Contains("não exist") => 404,
            var msg when msg.Contains("interno") => 500,
            var msg when msg.Contains("server") => 500,
            _ => 400
        };

        return statusCode switch
        {
            404 => NotFound(serviceResponse),
            500 => StatusCode(500, serviceResponse),
            _ => BadRequest(serviceResponse)
        };
    }

    /// <summary>
    /// Cria uma resposta de listagem paginada
    /// </summary>
    protected ActionResult<PagedResponseDto<T>> PagedResponse<T>(
        List<T> data,
        int totalCount,
        int pageNumber,
        int pageSize,
        string message = "Dados encontrados")
    {
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        var response = new PagedResponseDto<T>
        {
            Success = true,
            Message = message,
            Data = data,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = totalPages,
            HasPreviousPage = pageNumber > 1,
            HasNextPage = pageNumber < totalPages
        };

        return Ok(response);
    }
}

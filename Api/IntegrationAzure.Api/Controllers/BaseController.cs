using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using IntegrationAzure.Api.Application.DTOs;

namespace IntegrationAzure.Api.Controllers;

/// <summary>
/// Controller base com métodos utilitários comuns
/// Implementa padrões de resposta consistentes seguindo RESTful
/// </summary>
[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    /// <summary>
    /// Obtém o usuário atual do contexto HTTP
    /// Por enquanto retorna um valor mockado, mas pode ser integrado com autenticação
    /// NOTA: Implementar autenticação real quando necessário
    /// </summary>
    protected string GetCurrentUser()
    {
        // Retorna usuário do contexto de autenticação ou valor padrão do sistema
        // Pode ser expandido para integração com Azure AD, JWT ou outro provedor
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
        var response = new ApiResponseDto<T>
        {
            Success = false,
            Message = message,
            Errors = errors ?? new List<string>()
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

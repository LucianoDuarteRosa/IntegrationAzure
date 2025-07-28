using Microsoft.AspNetCore.Mvc;
using IntegrationAzure.Api.Application.Services;
using IntegrationAzure.Api.Application.DTOs;

namespace IntegrationAzure.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LogsController : BaseController
{
    private readonly LogService _logService;

    public LogsController(LogService logService)
    {
        _logService = logService;
    }

    /// <summary>
    /// Obtém logs com filtros
    /// </summary>
    /// <param name="userId">Filtrar por usuário</param>
    /// <param name="entity">Filtrar por entidade</param>
    /// <param name="level">Filtrar por nível</param>
    /// <param name="startDate">Data início</param>
    /// <param name="endDate">Data fim</param>
    /// <param name="pageSize">Tamanho da página</param>
    /// <param name="pageNumber">Número da página</param>
    /// <returns>Lista de logs</returns>
    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<List<LogSummaryDto>>>> GetLogs(
        [FromQuery] string? userId = null,
        [FromQuery] string? entity = null,
        [FromQuery] Domain.Entities.LogLevel? level = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int pageSize = 50,
        [FromQuery] int pageNumber = 1)
    {
        try
        {
            var filter = new LogFilterDto
            {
                UserId = userId,
                Entity = entity,
                Level = level,
                StartDate = startDate,
                EndDate = endDate,
                PageSize = pageSize,
                PageNumber = pageNumber
            };

            var result = await _logService.GetFilteredAsync(filter);
            return ProcessServiceResponse(result);
        }
        catch (Exception ex)
        {
            return ErrorResponse<List<LogSummaryDto>>(
                "Erro interno do servidor",
                new List<string> { ex.Message },
                500
            );
        }
    }

    /// <summary>
    /// Obtém logs recentes
    /// </summary>
    /// <param name="count">Número de logs a retornar</param>
    /// <returns>Lista de logs recentes</returns>
    [HttpGet("recent")]
    public async Task<ActionResult<ApiResponseDto<List<LogSummaryDto>>>> GetRecentLogs([FromQuery] int count = 100)
    {
        try
        {
            var result = await _logService.GetRecentAsync(count);
            return ProcessServiceResponse(result);
        }
        catch (Exception ex)
        {
            return ErrorResponse<List<LogSummaryDto>>(
                "Erro interno do servidor",
                new List<string> { ex.Message },
                500
            );
        }
    }

    /// <summary>
    /// Cria um novo log manual
    /// </summary>
    /// <param name="createDto">Dados do log</param>
    /// <returns>Log criado</returns>
    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<LogDto>>> CreateLog([FromBody] CreateLogDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationErrorResponse<LogDto>(ModelState);
            }

            var currentUser = GetCurrentUser();
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

            var result = await _logService.CreateAsync(createDto, currentUser, ipAddress, userAgent);
            return ProcessServiceResponse(result);
        }
        catch (Exception ex)
        {
            return ErrorResponse<LogDto>(
                "Erro interno do servidor",
                new List<string> { ex.Message },
                500
            );
        }
    }
}

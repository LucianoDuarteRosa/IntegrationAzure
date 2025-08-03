using IntegrationAzure.Api.Application.DTOs;
using IntegrationAzure.Api.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationAzure.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AzureDevOpsController : BaseController
{
    private readonly AzureDevOpsService _azureDevOpsService;

    public AzureDevOpsController(AzureDevOpsService azureDevOpsService)
    {
        _azureDevOpsService = azureDevOpsService;
    }

    [HttpGet("projects")]
    public async Task<ActionResult<ApiResponseDto<List<AzureProjectDto>>>> GetProjects()
    {
        try
        {
            var projects = await _azureDevOpsService.GetProjectsAsync();
            return SuccessResponse(projects, "Projetos recuperados com sucesso");
        }
        catch (InvalidOperationException ex)
        {
            return ErrorResponse<List<AzureProjectDto>>($"Configuração inválida: {ex.Message}");
        }
        catch (HttpRequestException ex)
        {
            return ErrorResponse<List<AzureProjectDto>>($"Erro na comunicação com Azure DevOps: {ex.Message}");
        }
        catch (Exception ex)
        {
            return ErrorResponse<List<AzureProjectDto>>($"Erro interno: {ex.Message}", null, 500);
        }
    }

    [HttpGet("projects/{projectId}/workitems")]
    public async Task<ActionResult<ApiResponseDto<List<AzureWorkItemDto>>>> GetWorkItems(string projectId, [FromQuery] string workItemType = "User Story")
    {
        try
        {
            if (string.IsNullOrEmpty(projectId))
            {
                return ErrorResponse<List<AzureWorkItemDto>>("ID do projeto é obrigatório");
            }

            var workItems = await _azureDevOpsService.GetWorkItemsAsync(projectId, workItemType);
            return SuccessResponse(workItems, $"Work items recuperados com sucesso do projeto {projectId}");
        }
        catch (InvalidOperationException ex)
        {
            return ErrorResponse<List<AzureWorkItemDto>>($"Configuração inválida: {ex.Message}");
        }
        catch (HttpRequestException ex)
        {
            return ErrorResponse<List<AzureWorkItemDto>>($"Erro na comunicação com Azure DevOps: {ex.Message}");
        }
        catch (Exception ex)
        {
            return ErrorResponse<List<AzureWorkItemDto>>($"Erro interno: {ex.Message}", null, 500);
        }
    }

    [HttpGet("projects/{projectId}/userstories")]
    public async Task<ActionResult<ApiResponseDto<List<AzureWorkItemDto>>>> GetUserStories(string projectId)
    {
        return await GetWorkItems(projectId, "User Story");
    }

    [HttpPost("projects/{projectId}/workitems")]
    public async Task<ActionResult<ApiResponseDto<AzureWorkItemDto>>> CreateWorkItem(
        string projectId,
        [FromBody] CreateAzureWorkItemDto createDto)
    {
        try
        {
            if (string.IsNullOrEmpty(projectId))
            {
                return ErrorResponse<AzureWorkItemDto>("ID do projeto é obrigatório");
            }

            if (createDto == null || string.IsNullOrEmpty(createDto.Title))
            {
                return ErrorResponse<AzureWorkItemDto>("Dados de entrada inválidos");
            }

            var workItem = await _azureDevOpsService.CreateWorkItemAsync(
                projectId,
                createDto.WorkItemType ?? "User Story",
                createDto.Title,
                createDto.Description ?? string.Empty,
                createDto.AdditionalFields
            );

            return SuccessResponse(workItem, $"Work item criado com sucesso no projeto {projectId}");
        }
        catch (InvalidOperationException ex)
        {
            return ErrorResponse<AzureWorkItemDto>($"Configuração inválida: {ex.Message}");
        }
        catch (HttpRequestException ex)
        {
            return ErrorResponse<AzureWorkItemDto>($"Erro na comunicação com Azure DevOps: {ex.Message}");
        }
        catch (Exception ex)
        {
            return ErrorResponse<AzureWorkItemDto>($"Erro interno: {ex.Message}", null, 500);
        }
    }

    [HttpGet("test-connection")]
    public async Task<ActionResult<ApiResponseDto<object>>> TestConnection()
    {
        try
        {
            var projects = await _azureDevOpsService.GetProjectsAsync();
            var projectCount = projects?.Count ?? 0;

            object result = new
            {
                IsConnected = true,
                ProjectCount = projectCount,
                Message = $"Conectado com sucesso. {projectCount} projeto(s) encontrado(s)."
            };

            return SuccessResponse(result, "Conexão com Azure DevOps testada com sucesso");
        }
        catch (InvalidOperationException ex)
        {
            return ErrorResponse<object>($"Configuração inválida: {ex.Message}");
        }
        catch (HttpRequestException ex)
        {
            return ErrorResponse<object>($"Erro na comunicação com Azure DevOps: {ex.Message}");
        }
        catch (Exception ex)
        {
            return ErrorResponse<object>($"Erro interno: {ex.Message}", null, 500);
        }
    }
}

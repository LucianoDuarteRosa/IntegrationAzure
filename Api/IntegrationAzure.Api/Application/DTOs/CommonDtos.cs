using IntegrationAzure.Api.Domain.Entities;

namespace IntegrationAzure.Api.Application.DTOs;

/// <summary>
/// DTO para criação de caso de teste
/// </summary>
public class CreateTestCaseDto
{
    public string Description { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
}

/// <summary>
/// DTO para retorno de caso de teste
/// </summary>
public class TestCaseDto
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
    public TestCaseStatus Status { get; set; }
    public string? Result { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}

/// <summary>
/// DTO para criação de anexo
/// </summary>
public class CreateAttachmentDto
{
    public string FileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long Size { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string? Description { get; set; }
}

/// <summary>
/// DTO para retorno de anexo
/// </summary>
public class AttachmentDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long Size { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}

/// <summary>
/// DTO para resposta padrão da API
/// </summary>
public class ApiResponseDto<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new();
}

/// <summary>
/// DTO para resposta de listagem com paginação
/// </summary>
public class PagedResponseDto<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<T> Data { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
}

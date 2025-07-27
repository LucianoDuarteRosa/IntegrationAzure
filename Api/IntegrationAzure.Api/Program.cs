using Microsoft.EntityFrameworkCore;
using FluentValidation;
using IntegrationAzure.Api.Infrastructure.Data;
using IntegrationAzure.Api.Domain.Interfaces;
using IntegrationAzure.Api.Infrastructure.Repositories;
using IntegrationAzure.Api.Application.Services;
using IntegrationAzure.Api.Application.Validators;
using IntegrationAzure.Api.Application.DTOs;
using IntegrationAzure.Api.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

// Configuração do PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Host=localhost;Database=integrationazure;Username=postgres;Password=postgres";

builder.Services.AddDbContext<IntegrationAzureDbContext>(options =>
    options.UseNpgsql(connectionString));

// Registro dos repositórios
builder.Services.AddScoped<IUserStoryRepository, UserStoryRepository>();
builder.Services.AddScoped<IIssueRepository, IssueRepository>();
builder.Services.AddScoped<IFailureRepository, FailureRepository>();
builder.Services.AddScoped<IRepository<TestCase>, Repository<TestCase>>();
builder.Services.AddScoped<IRepository<Attachment>, Repository<Attachment>>();

// Registro dos serviços de aplicação
builder.Services.AddScoped<UserStoryService>();
builder.Services.AddScoped<IssueService>();
builder.Services.AddScoped<FailureService>();

// Registro dos validadores
builder.Services.AddScoped<IValidator<CreateUserStoryDto>, CreateUserStoryDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateUserStoryDto>, UpdateUserStoryDtoValidator>();
builder.Services.AddScoped<IValidator<CreateIssueDto>, CreateIssueDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateIssueDto>, UpdateIssueDtoValidator>();
builder.Services.AddScoped<IValidator<CreateFailureDto>, CreateFailureDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateFailureDto>, UpdateFailureDtoValidator>();

// Configuração de Controllers
builder.Services.AddControllers();

// Configuração de CORS para o frontend React
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000") // Vite e Create React App
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Configuração do FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserStoryDtoValidator>();

// Configuração do Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Integration Azure API",
        Version = "v1",
        Description = "API para integração com Azure - Gerenciamento de Histórias, Issues e Falhas",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Integration Azure Team",
            Email = "team@integrationazure.com"
        }
    });

    // Incluir comentários XML para documentação
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Configuração do pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Integration Azure API v1");
        c.RoutePrefix = string.Empty; // Para acessar o Swagger na raiz
    });

    // Aplicar migrações automaticamente em desenvolvimento
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<IntegrationAzureDbContext>();
        context.Database.EnsureCreated(); // Para desenvolvimento - em produção use Migrate()
    }
}

// Middlewares
app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.UseRouting();

// Mapeamento dos controllers
app.MapControllers();

// Endpoint de health check
app.MapGet("/health", () => new
{
    Status = "Healthy",
    Timestamp = DateTime.UtcNow,
    Version = "1.0.0"
})
.WithName("HealthCheck")
.WithTags("Health");

app.Run();

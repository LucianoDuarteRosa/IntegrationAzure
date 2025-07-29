using Microsoft.EntityFrameworkCore;
using IntegrationAzure.Api.Domain.Entities;
using IntegrationAzure.Api.Infrastructure.Data;

namespace IntegrationAzure.Api.Infrastructure.Data;

/// <summary>
/// Classe responsável por inserir dados iniciais no banco de dados
/// </summary>
public static class DataSeeder
{
    /// <summary>
    /// Popula os perfis padrão do sistema
    /// </summary>
    public static async Task SeedProfilesAsync(IntegrationAzureDbContext context)
    {
        // Verificar se já existem perfis
        if (await context.Profiles.AnyAsync())
        {
            return; // Já existem perfis, não precisa popular
        }

        var profiles = new List<Profile>
        {
            new Profile
            {
                Id = Guid.NewGuid(),
                Name = "Administrador",
                Description = "Perfil com acesso total ao sistema, pode gerenciar usuários, configurações e todas as funcionalidades",
                CreatedBy = "system@integrationazure.com",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new Profile
            {
                Id = Guid.NewGuid(),
                Name = "Desenvolvedor",
                Description = "Perfil para desenvolvedores com acesso a funcionalidades de desenvolvimento, criação de user stories e gerenciamento de issues",
                CreatedBy = "system@integrationazure.com",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new Profile
            {
                Id = Guid.NewGuid(),
                Name = "Usuário",
                Description = "Perfil básico com acesso limitado às funcionalidades essenciais do sistema",
                CreatedBy = "system@integrationazure.com",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            }
        };

        await context.Profiles.AddRangeAsync(profiles);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Popula todos os dados iniciais
    /// </summary>
    public static async Task SeedAllAsync(IntegrationAzureDbContext context)
    {
        await SeedProfilesAsync(context);
    }
}

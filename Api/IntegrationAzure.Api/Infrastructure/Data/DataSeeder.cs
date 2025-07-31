using Microsoft.EntityFrameworkCore;
using IntegrationAzure.Api.Domain.Entities;
using IntegrationAzure.Api.Infrastructure.Data;
using System.Security.Cryptography;
using System.Text;

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
    /// Popula um usuário administrador padrão do sistema
    /// </summary>
    public static async Task SeedAdminUserAsync(IntegrationAzureDbContext context)
    {
        // Verificar se já existe o usuário admin
        if (await context.Users.AnyAsync(u => u.Email == "admin@admin.com"))
        {
            return; // Usuário admin já existe
        }

        // Buscar o perfil de Desenvolvedor
        var developerProfile = await context.Profiles.FirstOrDefaultAsync(p => p.Name == "Desenvolvedor");
        if (developerProfile == null)
        {
            throw new InvalidOperationException("Perfil 'Desenvolvedor' não encontrado. Execute o seed de profiles primeiro.");
        }

        var adminUser = new User
        {
            Id = Guid.NewGuid(),
            Name = "Administrador",
            Nickname = "admin",
            Email = "admin@admin.com",
            Password = HashPassword("123456"), // Hash da senha 123456
            ProfileId = developerProfile.Id,
            CreatedBy = "system@integrationazure.com",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await context.Users.AddAsync(adminUser);
        await context.SaveChangesAsync();
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
    /// Popula todos os dados iniciais
    /// </summary>
    public static async Task SeedAllAsync(IntegrationAzureDbContext context)
    {
        await SeedProfilesAsync(context);
        await SeedAdminUserAsync(context);
    }
}

using Microsoft.EntityFrameworkCore;
using IntegrationAzure.Api.Domain.Entities;

namespace IntegrationAzure.Api.Infrastructure.Data;

/// <summary>
/// Contexto do banco de dados para a aplicação
/// Configurado para PostgreSQL seguindo boas práticas do EF Core
/// </summary>
public class IntegrationAzureDbContext : DbContext
{
    public IntegrationAzureDbContext(DbContextOptions<IntegrationAzureDbContext> options)
        : base(options)
    {
    }

    // DbSets para as entidades
    public DbSet<UserStory> UserStories { get; set; }
    public DbSet<Issue> Issues { get; set; }
    public DbSet<Failure> Failures { get; set; }
    public DbSet<Attachment> Attachments { get; set; }
    public DbSet<Log> Logs { get; set; }
    public DbSet<Configuration> Configurations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurações para UserStory
        modelBuilder.Entity<UserStory>(entity =>
        {
            entity.ToTable("user_stories");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DemandNumber).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Title).HasMaxLength(255).IsRequired();
            entity.Property(e => e.AcceptanceCriteria).IsRequired();
            entity.Property(e => e.CreatedBy).HasMaxLength(100).IsRequired();

            // Índices para performance
            entity.HasIndex(e => e.DemandNumber);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.Priority);
            entity.HasIndex(e => e.CreatedAt);
        });

        // Configurações para Issue
        modelBuilder.Entity<Issue>(entity =>
        {
            entity.ToTable("issues");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.IssueNumber).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Title).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.AssignedTo).HasMaxLength(100);
            entity.Property(e => e.Reporter).HasMaxLength(100);
            entity.Property(e => e.CreatedBy).HasMaxLength(100).IsRequired();
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);

            // Relacionamento com UserStory (opcional)
            entity.HasOne(e => e.UserStory)
                  .WithMany()
                  .HasForeignKey(e => e.UserStoryId)
                  .OnDelete(DeleteBehavior.SetNull);

            // Índices para performance
            entity.HasIndex(e => e.IssueNumber);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.Type);
            entity.HasIndex(e => e.Priority);
            entity.HasIndex(e => e.AssignedTo);
        });

        // Configurações para Failure
        modelBuilder.Entity<Failure>(entity =>
        {
            entity.ToTable("failures");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FailureNumber).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Title).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.ReportedBy).HasMaxLength(100);
            entity.Property(e => e.AssignedTo).HasMaxLength(100);
            entity.Property(e => e.CreatedBy).HasMaxLength(100).IsRequired();
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.EstimatedImpactCost).HasColumnType("decimal(18,2)");

            // Relacionamentos opcionais
            entity.HasOne(e => e.Issue)
                  .WithMany()
                  .HasForeignKey(e => e.IssueId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.UserStory)
                  .WithMany()
                  .HasForeignKey(e => e.UserStoryId)
                  .OnDelete(DeleteBehavior.SetNull);

            // Índices para performance
            entity.HasIndex(e => e.FailureNumber);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.Severity);
            entity.HasIndex(e => e.OccurredAt);
            entity.HasIndex(e => e.AssignedTo);
        });

        // Configurações para Attachment
        modelBuilder.Entity<Attachment>(entity =>
        {
            entity.ToTable("attachments");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FileName).HasMaxLength(255).IsRequired();
            entity.Property(e => e.OriginalFileName).HasMaxLength(255).IsRequired();
            entity.Property(e => e.ContentType).HasMaxLength(100).IsRequired();
            entity.Property(e => e.FilePath).IsRequired();
            entity.Property(e => e.CreatedBy).HasMaxLength(100).IsRequired();
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);

            // Relacionamentos opcionais
            entity.HasOne(e => e.UserStory)
                  .WithMany(us => us.Attachments)
                  .HasForeignKey(e => e.UserStoryId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Issue)
                  .WithMany(i => i.Attachments)
                  .HasForeignKey(e => e.IssueId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Failure)
                  .WithMany(f => f.Attachments)
                  .HasForeignKey(e => e.FailureId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Índices para performance
            entity.HasIndex(e => e.UserStoryId);
            entity.HasIndex(e => e.IssueId);
            entity.HasIndex(e => e.FailureId);
        });

        // Configurações adicionais para enums (usando conversão para string para melhor legibilidade)
        modelBuilder.Entity<UserStory>()
            .Property(e => e.Status)
            .HasConversion<string>();

        modelBuilder.Entity<UserStory>()
            .Property(e => e.Priority)
            .HasConversion<string>();

        modelBuilder.Entity<Issue>()
            .Property(e => e.Type)
            .HasConversion<string>();

        modelBuilder.Entity<Issue>()
            .Property(e => e.Status)
            .HasConversion<string>();

        modelBuilder.Entity<Issue>()
            .Property(e => e.Priority)
            .HasConversion<string>();

        modelBuilder.Entity<Failure>()
            .Property(e => e.Severity)
            .HasConversion<string>();

        modelBuilder.Entity<Failure>()
            .Property(e => e.Status)
            .HasConversion<string>();

        // Configurações para Log
        modelBuilder.Entity<Log>(entity =>
        {
            entity.ToTable("logs");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Action).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Entity).HasMaxLength(100).IsRequired();
            entity.Property(e => e.EntityId).HasMaxLength(100);
            entity.Property(e => e.UserId).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Details).HasMaxLength(500);
            entity.Property(e => e.IpAddress).HasMaxLength(50);
            entity.Property(e => e.UserAgent).HasMaxLength(500);
            entity.Property(e => e.CreatedBy).HasMaxLength(100).IsRequired();
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);

            // Índices para performance
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Entity);
            entity.HasIndex(e => e.Level);
            entity.HasIndex(e => e.CreatedAt);
        });

        // Configuração de enum para Log
        modelBuilder.Entity<Log>()
            .Property(e => e.Level)
            .HasConversion<string>();

        // Configurações para Configuration
        modelBuilder.Entity<Configuration>(entity =>
        {
            entity.ToTable("configurations");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Key).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Value).HasMaxLength(1000).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).HasMaxLength(100).IsRequired();
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);

            // Índices para performance e unicidade
            entity.HasIndex(e => e.Key).IsUnique();
            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.CreatedAt);
        });
    }

    /// <summary>
    /// Override para aplicar auditoria automática nas entidades
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Aplicar auditoria automática
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    // O CreatedBy deve ser definido na camada de aplicação
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    // O UpdatedBy deve ser definido na camada de aplicação
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}

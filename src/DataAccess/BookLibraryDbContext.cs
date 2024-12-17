using DataAccess.Configurations;
using DataExplorer;
using DataExplorer.Abstractions.Entities;
using DataExplorer.EfCore;
using DataExplorer.EfCore.DataContexts;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Options;

namespace DataAccess;

/// <inheritdoc cref="IBookLibraryDbContext"/>
/// <inheritdoc cref="EfDbContext"/>
public sealed class BookLibraryDbContext : EfDbContext, IBookLibraryDbContext
{
    public BookLibraryDbContext(DbContextOptions<BookLibraryDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSnakeCaseNamingConvention();
        
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderDetailConfiguration).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    protected override ValueTask OnBeforeSaveChangesAsync(IReadOnlyList<EntityEntry>? entries = null)
    {
        entries ??= GetTrackedEntries();
        
        var nowOffset = Config.Value.DateTimeStrategy switch
        {
            DateTimeStrategy.UtcNow => TimeProvider.GetUtcNow(),
            DateTimeStrategy.Now => TimeProvider.GetLocalNow(),
            _ => throw new ArgumentOutOfRangeException()
        };
        
        var now = Config.Value.DateTimeStrategy switch
        {
            DateTimeStrategy.UtcNow => nowOffset.DateTime.ToUniversalTime(),
            DateTimeStrategy.Now => nowOffset.DateTime.ToLocalTime(),
            _ => throw new ArgumentOutOfRangeException()
        };

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    if (entry.Entity is ICreatedAt { CreatedAt: null } createdAt)
                    {
                        createdAt.CreatedAt = now;
                        entry.Property(nameof(ICreatedAt.CreatedAt)).IsModified = true;
                    }

                    if (entry.Entity is ICreatedAtOffset { CreatedAt: null } createdAtOffset)
                    {
                        createdAtOffset.CreatedAt = nowOffset;
                        entry.Property(nameof(ICreatedAtOffset.CreatedAt)).IsModified = true;
                    }

                    if (entry.Entity is IUpdatedAt { UpdatedAt: null } addedUpdatedAt)
                    {
                        addedUpdatedAt.UpdatedAt = now;
                        entry.Property(nameof(IUpdatedAt.UpdatedAt)).IsModified = true;
                    }

                    if (entry.Entity is IUpdatedAtOffset { UpdatedAt: null } addedUpdatedAtOffset)
                    {
                        addedUpdatedAtOffset.UpdatedAt = nowOffset;
                        entry.Property(nameof(IUpdatedAtOffset.UpdatedAt)).IsModified = true;
                    }

                    break;
                case EntityState.Modified:
                    if (entry.Entity is IUpdatedAt updatedAt &&
                        !entry.Property(nameof(IUpdatedAt.UpdatedAt)).IsModified)
                    {
                        updatedAt.UpdatedAt = now;
                        entry.Property(nameof(IUpdatedAt.UpdatedAt)).IsModified = true;
                    }

                    if (entry.Entity is IUpdatedAtOffset updatedAtOffset &&
                        !entry.Property(nameof(IUpdatedAt.UpdatedAt)).IsModified)
                    {
                        updatedAtOffset.UpdatedAt = nowOffset;
                        entry.Property(nameof(IUpdatedAtOffset.UpdatedAt)).IsModified = true;
                    }

                    break;
            }
        }
        
        return ValueTask.CompletedTask;
    }
}
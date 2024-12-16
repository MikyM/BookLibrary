using DataAccess.Configurations;
using DataExplorer;
using DataExplorer.EfCore;
using DataExplorer.EfCore.DataContexts;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DataAccess;

/// <inheritdoc cref="IBookLibraryDbContext"/>
/// <inheritdoc cref="EfDbContext"/>
public sealed class BookLibraryDbContext : EfDbContext, IBookLibraryDbContext
{
    public BookLibraryDbContext(DbContextOptions options) : base(options)
    {
    }

    public BookLibraryDbContext(DbContextOptions options, IOptions<DataExplorerEfCoreConfiguration> config, DataExplorerTimeProvider timeProvider) : base(options, config, timeProvider)
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
}
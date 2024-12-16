using DataExplorer.EfCore.Abstractions.DataContexts;

namespace DataAccess;

/// <summary>
/// Represents the context of the database.
/// </summary>
/// <inheritdoc cref="IEfDbContext"/>
public interface IBookLibraryDbContext : IEfDbContext
{
}
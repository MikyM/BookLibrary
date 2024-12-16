using DataExplorer.Abstractions.Entities;
using DataExplorer.Entities;

namespace Domain.Base;

/// <summary>
/// Defines the base entity.
/// </summary>
/// <typeparam name="TId">The type of the ID.</typeparam>
/// <inheritdoc cref="Entity{TId}"/>
/// <inheritdoc cref="ICreatedAtOffset"/>
/// <inheritdoc cref="IUpdatedAtOffset"/>
public abstract class EntityBase<TId> : Entity<TId>, ICreatedAtOffset, IUpdatedAtOffset where TId : IComparable, IComparable<TId>, IEquatable<TId>
{
    /// <inheritdoc/>
    public DateTimeOffset? CreatedAt { get; set; }
    /// <inheritdoc/>
    public DateTimeOffset? UpdatedAt { get; set; }
}
using DataExplorer.Abstractions.Entities;

namespace Domain.Base;

public interface IEntityBase<TId> : IEntity<TId>, ICreatedAtOffset, IUpdatedAtOffset where TId : IComparable, IComparable<TId>, IEquatable<TId>
{
}
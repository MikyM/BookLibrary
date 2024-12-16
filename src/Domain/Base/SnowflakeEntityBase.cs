using DataExplorer.Abstractions.Entities;
using DataExplorer.IdGenerator;

namespace Domain.Base;

public abstract class SnowflakeEntityBase : EntityBase<long>, ISnowflakeEntity
{
    /// <summary>
    /// The ID of the entity.
    /// </summary>
    public override long Id { get; protected set; } = SnowflakeIdFactory.CreateId();
}
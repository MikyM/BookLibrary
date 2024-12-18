using DataExplorer.EfCore.Specifications;
using Domain.Base;

namespace DataAccess.Specifications.Book;

public class EntityProjectedSpecification<TEntity,TId,TProjection> : Specification<TEntity, TProjection> where TEntity : EntityBase<TId> where TId : IComparable, IComparable<TId>, IEquatable<TId>
{
    public EntityProjectedSpecification(TId id)
    {
        Where(b => b.Id.Equals(id));
    }

    public EntityProjectedSpecification()
    {
    }
}
using Domain.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations;

public abstract class BaseEntityConfiguration<TEntity,TId> : IEntityTypeConfiguration<TEntity> where TEntity : EntityBase<TId> where TId : IComparable, IComparable<TId>, IEquatable<TId>
{
    public void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(x => x.Id);
    }
    
    public abstract void ConfigureEntity(EntityTypeBuilder<TEntity> builder);
}
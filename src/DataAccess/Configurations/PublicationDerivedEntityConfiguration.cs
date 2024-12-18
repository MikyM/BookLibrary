using Domain.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations;

public abstract class PublicationDerivedEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : Publication
{
    public void Configure(EntityTypeBuilder<TEntity> builder)
    {
        ConfigureEntity(builder);
    }
    
    protected abstract void ConfigureEntity(EntityTypeBuilder<TEntity> builder);
}
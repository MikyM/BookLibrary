using Domain;
using Domain.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations;

public abstract class PublicationDerivedEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : Publication
{
    public void Configure(EntityTypeBuilder<TEntity> builder)
    {
        /*builder.Metadata.FindNavigation(nameof(Publication.Authors))!.SetPropertyAccessMode(PropertyAccessMode.Field);
        builder.Metadata.FindNavigation(nameof(Publication.PublicationAuthors))!.SetPropertyAccessMode(PropertyAccessMode.Field);
        builder.Metadata.FindNavigation(nameof(Publication.Orders))!.SetPropertyAccessMode(PropertyAccessMode.Field);
        builder.Metadata.FindNavigation(nameof(Publication.OrderDetails))!.SetPropertyAccessMode(PropertyAccessMode.Field);*/
        
        ConfigureEntity(builder);
    }
    
    protected abstract void ConfigureEntity(EntityTypeBuilder<TEntity> builder);
}
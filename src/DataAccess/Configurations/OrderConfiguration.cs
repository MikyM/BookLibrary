using Domain;
using Domain.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations;

public class OrderConfiguration : BaseEntityConfiguration<Order,Guid>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("order");
        
        builder.HasMany(x => x.Publications)
            .WithMany(x => x.Orders)
            .UsingEntity<OrderDetail>(r => r.HasOne<Publication>(x => x.Publication)
                    .WithMany(x => x.OrderDetails)
                    .HasForeignKey(x => x.PublicationId)
                    .HasPrincipalKey(x => x.Id)
                    .IsRequired(),
                l => l.HasOne<Order>(x => x.Order)
                    .WithMany(x => x.OrderDetails)
                    .HasForeignKey(x => x.OrderId)
                    .HasPrincipalKey(x => x.Id)
                    .IsRequired());
    }
}
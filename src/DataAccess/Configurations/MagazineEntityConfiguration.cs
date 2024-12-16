using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations;

public class MagazineEntityConfiguration : BaseEntityConfiguration<Magazine,long>
{
    public override void ConfigureEntity(EntityTypeBuilder<Magazine> builder)
    {
        builder.ToTable("magazine");
    }
}
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations;

public class MagazineConfiguration : PublicationDerivedEntityConfiguration<Magazine>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Magazine> builder)
    {
        builder.ToTable("magazine");
    }
}
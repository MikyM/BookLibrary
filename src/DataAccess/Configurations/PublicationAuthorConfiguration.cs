using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations;

public class PublicationAuthorEntityConfiguration : IEntityTypeConfiguration<PublicationAuthor>
{
    public void Configure(EntityTypeBuilder<PublicationAuthor> builder)
    {
        builder.ToTable("publication_author");
    }
}
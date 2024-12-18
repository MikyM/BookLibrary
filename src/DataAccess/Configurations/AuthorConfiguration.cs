using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations;

public class AuthorConfiguration : BaseEntityConfiguration<Author,long>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Author> builder)
    {
        builder.ToTable("author");
    }
}
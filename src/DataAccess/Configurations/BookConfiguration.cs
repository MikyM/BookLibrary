using Domain;
using Domain.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations;

public class BookConfiguration : PublicationDerivedEntityConfiguration<Book>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable("book");
    }
}
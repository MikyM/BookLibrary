using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations;

public class BookEntityConfiguration : BaseEntityConfiguration<Book,long>
{
    public override void ConfigureEntity(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable("book");
    }
}
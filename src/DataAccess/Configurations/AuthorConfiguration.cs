using Domain;
using Domain.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations;

public class AuthorConfiguration : BaseEntityConfiguration<Author,long>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Author> builder)
    {
        builder.ToTable("author");
        
        /*builder.Metadata.FindNavigation(nameof(Author.Books))!.SetPropertyAccessMode(PropertyAccessMode.Field);
        builder.Metadata.FindNavigation(nameof(Author.Magazines))!.SetPropertyAccessMode(PropertyAccessMode.Field);*/
    }
}
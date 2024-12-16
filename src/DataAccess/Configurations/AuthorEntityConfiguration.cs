using Domain;
using Domain.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations;

public class AuthorEntityConfiguration : BaseEntityConfiguration<Author,long>
{
    public override void ConfigureEntity(EntityTypeBuilder<Author> builder)
    {
        builder.ToTable("author");

        builder.HasMany(x => x.Books)
            .WithMany(x => x.Authors)
            .UsingEntity<PublicationAuthor>(
                r => r.HasOne<Book>(x => (Book?)x.Publication)
                    .WithMany(x => x.PublicationAuthors)
                    .HasForeignKey(x => x.PublicationId)
                    .HasPrincipalKey(x => x.Id)
                    .IsRequired(),
                l => l.HasOne<Author>(x => x.Author)
                    .WithMany(x => x.PublicationAuthors)
                    .HasForeignKey(x => x.Publication)
                    .HasPrincipalKey(x => x.Id)
                    .IsRequired());

        builder.HasMany(x => x.Magazines)
            .WithMany(x => x.Authors)
            .UsingEntity<PublicationAuthor>(
                r => r.HasOne<Magazine>(x => (Magazine?)x.Publication)
                    .WithMany(x => x.PublicationAuthors)
                    .HasForeignKey(x => x.PublicationId)
                    .HasPrincipalKey(x => x.Id)
                    .IsRequired(),
                l => l.HasOne<Author>(x => x.Author)
                    .WithMany(x => x.PublicationAuthors)
                    .HasForeignKey(x => x.Author)
                    .HasPrincipalKey(x => x.Id)
                    .IsRequired());
    }
}
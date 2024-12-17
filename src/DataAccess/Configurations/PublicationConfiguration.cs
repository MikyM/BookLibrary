using Domain;
using Domain.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations;

public class PublicationConfiguration : BaseEntityConfiguration<Publication,long>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Publication> builder)
    {
        builder.UseTpcMappingStrategy();
        
        builder.HasMany(x => x.Authors)
            .WithMany(x => x.Publications)
            .UsingEntity<PublicationAuthor>(r => r.HasOne(x => x.Author)
                    .WithMany(x => x.PublicationAuthors)
                    .HasForeignKey(x => x.AuthorId)
                    .HasPrincipalKey(x => x.Id)
                    .IsRequired(),
                l => l.HasOne(x => x.Publication)
                    .WithMany(x => x.PublicationAuthors)
                    .HasForeignKey(x => x.PublicationId)
                    .HasPrincipalKey(x => x.Id)
                    .IsRequired());
    }
}
using Domain.Base;

namespace DataAccess.Specifications.Book;

public class PublicationWithAuthorsSpecification<TPublication,TProjectedPayload> : EntityProjectedSpecification<TPublication,long,TProjectedPayload> where TPublication : Publication
{
    public PublicationWithAuthorsSpecification(long id) : base(id)
    {
        Configure();
    }

    public PublicationWithAuthorsSpecification()
    {
        Configure();
    }

    private void Configure()
    {
        Include(x => x.Authors);
    }
}
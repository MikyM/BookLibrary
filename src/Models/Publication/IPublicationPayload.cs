using Models.Author;

namespace Models.Book;

public interface IPublicationPayload
{
    public long Id { get; }
    
    public IEnumerable<IAuthorPayload> Authors { get; }

    public string Title { get; }
    
    public decimal Price { get; }
    
    public int Bookstand { get; }
    
    public int Shelf { get; }
}
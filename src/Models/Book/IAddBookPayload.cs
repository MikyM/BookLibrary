using Models.Author;

namespace Models.Book;

public interface IAddBookPayload
{
    public IEnumerable<AuthorPayload> Authors { get; }

    public string Title { get; }
    
    public decimal Price { get; }
    
    public int Bookstand { get; }
    
    public int Shelf { get; }
}
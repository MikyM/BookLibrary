using Models.Author;

namespace Models.Book;

public record AddBookPayload(IEnumerable<AuthorPayload> Authors, string Title, decimal Price, int Bookstand, int Shelf) : IAddBookPayload;
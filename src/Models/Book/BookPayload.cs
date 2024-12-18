using Models.Author;

namespace Models.Book;

public record BookPayload(long Id, IEnumerable<IAuthorPayload> Authors, string Title, decimal Price, int Bookstand, int Shelf) : PublicationPayload(Id, Authors, Title, Price, Bookstand, Shelf), IBookPayload;
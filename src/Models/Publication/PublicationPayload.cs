using Models.Author;

namespace Models.Book;

public record PublicationPayload(long Id, IEnumerable<IAuthorPayload> Authors, string Title, decimal Price, int Bookstand, int Shelf) : IPublicationPayload;
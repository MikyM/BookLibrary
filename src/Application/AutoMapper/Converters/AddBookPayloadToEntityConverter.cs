using AutoMapper;
using Models.Book;

namespace Application.AutoMapper.Converters;

[UsedImplicitly]
public class AddBookPayloadToEntityConverter : ITypeConverter<IAddBookPayload,Book>
{
    public Book Convert(IAddBookPayload source, Book destination, ResolutionContext context)
    {
        var book = new Book()
        {
            Title = source.Title,
            Price = source.Price,
            Bookstand = source.Bookstand,
            Shelf = source.Shelf
        };
        
        book.WithAuthors(context.Mapper.Map<IEnumerable<Author>>(source.Authors));

        return book;
    }
}
using Application.AutoMapper.Converters;
using AutoMapper;
using Models.Author;
using Models.Book;

namespace Application.AutoMapper.Profiles;

public class BookProfile : Profile
{
    public BookProfile()
    {
        CreateMap<Book, BookPayload>()
            .ConstructUsing(x => new BookPayload(x.Id,
                x.Authors!.Select(y => new AuthorPayload(y.FirstName, y.LastName)), x.Title, x.Price, x.Bookstand,
                x.Shelf))
            .ReverseMap();

        CreateMap<Book, IBookPayload>()
            .ConstructUsing(x => new BookPayload(x.Id,
                x.Authors!.Select(y => new AuthorPayload(y.FirstName, y.LastName)), x.Title, x.Price, x.Bookstand,
                x.Shelf))
            .ReverseMap();
        
        CreateMap<IAddBookPayload, Book>()
            .ConvertUsing<AddBookPayloadToEntityConverter>();
    }
}
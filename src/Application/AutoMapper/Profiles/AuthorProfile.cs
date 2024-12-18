using AutoMapper;
using Models.Author;

namespace Application.AutoMapper.Profiles;

public class AuthorProfile : Profile
{
    public AuthorProfile()
    {
        CreateMap<Author, AuthorPayload>()
            .ConstructUsing(x => new AuthorPayload(x.FirstName, x.LastName))
            .ReverseMap();

        CreateMap<Author, IAuthorPayload>()
            .ConstructUsing(x => new AuthorPayload(x.FirstName, x.LastName))
            .ReverseMap();
        
        CreateMap<IAuthorPayload, Author>()
            .ConstructUsing((x, y) => y.Mapper.Map<Author>(x));
    }
}
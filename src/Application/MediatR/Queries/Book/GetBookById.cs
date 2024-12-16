using Application.Models.Book;
using MediatR;
using Remora.Results;

namespace Application.MediatR.Queries.Book;

public static class GetBookById
{
    public sealed record Query(long Id) : IQuery<IBook>;

    internal sealed class Handler : IRequestHandler<Query, Result<IBook>>
    {
        public Task<Result<IBook>> Handle(Query request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
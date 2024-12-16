using Application.Models.Book;
using Gridify;
using MediatR;
using Remora.Results;

namespace Application.MediatR.Queries.Book;

public static class GetBooksByGridifyQuery
{
    public sealed record Query(GridifyQuery GridifyQuery) : IQuery<Paging<IBook>>;

    internal sealed class Handler : IRequestHandler<Query, Result<Paging<IBook>>>
    {
        public Task<Result<Paging<IBook>>> Handle(Query request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
using DataAccess;
using DataAccess.Specifications.Book;
using DataExplorer.EfCore.Abstractions.DataServices;
using MediatR;
using Models.Book;
using Remora.Results;

namespace Application.MediatR.Queries.Book;

public static class GetAllBooks
{
    public sealed record Query() : IQuery<IReadOnlyList<IBookPayload>>;

    [UsedImplicitly]
    public sealed class Handler : IRequestHandler<Query, Result<IReadOnlyList<IBookPayload>>>
    {
        private readonly IReadOnlyDataService<Domain.Book,IBookLibraryDbContext> _dataService;

        public Handler(IReadOnlyDataService<Domain.Book, IBookLibraryDbContext> dataService)
        {
            _dataService = dataService;
        }

        public async Task<Result<IReadOnlyList<IBookPayload>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var getResult =
                await _dataService.GetAsync(new EntityProjectedSpecification<Domain.Book, long, BookPayload>(), cancellationToken);

            return !getResult.IsDefined(out var entity) 
                ? Result<IReadOnlyList<IBookPayload>>.FromError(getResult) 
                : Result<IReadOnlyList<IBookPayload>>.FromSuccess(entity);
        }
    }
}
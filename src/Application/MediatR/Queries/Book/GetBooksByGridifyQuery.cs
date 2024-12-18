using Application.Extensions;
using DataAccess;
using DataExplorer.EfCore.Abstractions.DataServices;
using Gridify;
using MediatR;
using Models;
using Models.Book;
using Remora.Results;

namespace Application.MediatR.Queries.Book;

public static class GetBooksByGridifyQuery
{
    public sealed record Query(GridifyQuery GridifyQuery) : IQuery<IExtendedPaging<IBookPayload>>;

    [UsedImplicitly]
    public sealed class Handler : IRequestHandler<Query, Result<IExtendedPaging<IBookPayload>>>
    {
        private readonly IReadOnlyDataService<Domain.Book,IBookLibraryDbContext> _dataService;

        public Handler(IReadOnlyDataService<Domain.Book, IBookLibraryDbContext> dataService)
        {
            _dataService = dataService;
        }

        public async Task<Result<IExtendedPaging<IBookPayload>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var getResult = await _dataService.GetAsync<Domain.Book,long,BookPayload,IBookPayload>(request.GridifyQuery, cancellationToken);

            return !getResult.IsDefined(out var entity) 
                ? Result<IExtendedPaging<IBookPayload>>.FromError(getResult) 
                : Result<IExtendedPaging<IBookPayload>>.FromSuccess(entity);
        }
    }
}
using Application.Extensions;
using DataAccess;
using DataExplorer.EfCore.Abstractions.DataServices;
using Gridify;
using MediatR;
using Models;
using Models.Author;
using Remora.Results;

namespace Application.MediatR.Queries.Order;

public static class GetAuthorsByGridifyQuery
{
    public sealed record Query(GridifyQuery GridifyQuery) : IQuery<IExtendedPaging<IAuthorPayload>>;

    public sealed class Handler : IRequestHandler<Query, Result<IExtendedPaging<IAuthorPayload>>>
    {
        private readonly IReadOnlyDataService<Domain.Author, long, IBookLibraryDbContext> _dataService;

        public Handler(IReadOnlyDataService<Domain.Author, long, IBookLibraryDbContext> dataService)
        {
            _dataService = dataService;
        }

        public async Task<Result<IExtendedPaging<IAuthorPayload>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var getResult = await _dataService.GetAsync<Domain.Author, long, AuthorPayload, IAuthorPayload>(request.GridifyQuery, cancellationToken);

            return !getResult.IsDefined(out var entity) 
                ? Result<IExtendedPaging<IAuthorPayload>>.FromError(getResult) 
                : Result<IExtendedPaging<IAuthorPayload>>.FromSuccess(entity);
        }
    }
}
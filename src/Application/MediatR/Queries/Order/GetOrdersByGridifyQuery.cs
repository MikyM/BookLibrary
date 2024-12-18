using Application.Extensions;
using DataAccess;
using DataExplorer.EfCore.Abstractions.DataServices;
using Gridify;
using MediatR;
using Models;
using Models.Order;
using Remora.Results;

namespace Application.MediatR.Queries.Order;

public static class GetOrdersByGridifyQuery
{
    public sealed record Query(GridifyQuery GridifyQuery) : IQuery<IExtendedPaging<IOrderPayload>>;

    public sealed class Handler : IRequestHandler<Query, Result<IExtendedPaging<IOrderPayload>>>
    {
        private readonly IReadOnlyDataService<Domain.Order, Guid, IBookLibraryDbContext> _dataService;

        public Handler(IReadOnlyDataService<Domain.Order, Guid, IBookLibraryDbContext> dataService)
        {
            _dataService = dataService;
        }

        public async Task<Result<IExtendedPaging<IOrderPayload>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var getResult = await _dataService.GetAsync<Domain.Order, Guid, OrderPayload,IOrderPayload>(request.GridifyQuery, cancellationToken);

            return !getResult.IsDefined(out var entity) 
                ? Result<IExtendedPaging<IOrderPayload>>.FromError(getResult) 
                : Result<IExtendedPaging<IOrderPayload>>.FromSuccess(entity);
        }
    }
}
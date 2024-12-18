using DataAccess;
using DataAccess.Specifications.Book;
using DataExplorer.EfCore.Abstractions.DataServices;
using MediatR;
using Models.Order;
using Remora.Results;

namespace Application.MediatR.Queries.Order;

public static class GetOrderById
{
    public sealed record Query(Guid Id) : IQuery<IOrderPayload>;

    public sealed class Handler : IRequestHandler<Query, Result<IOrderPayload>>
    {
        private readonly IReadOnlyDataService<Domain.Order, Guid, IBookLibraryDbContext> _dataService;

        public Handler(IReadOnlyDataService<Domain.Order, Guid, IBookLibraryDbContext> dataService)
        {
            _dataService = dataService;
        }

        public async Task<Result<IOrderPayload>> Handle(Query request, CancellationToken cancellationToken)
        {
            var spec = new EntityProjectedSpecification<Domain.Order,Guid,OrderPayload>(request.Id);
            
            var getResult = await _dataService.GetSingleAsync(spec, cancellationToken);

            return !getResult.IsDefined(out var entity) 
                ? Result<IOrderPayload>.FromError(getResult) 
                : entity;
        }
    }
}
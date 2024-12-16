using Application.Models.Order;
using Gridify;
using MediatR;
using Remora.Results;

namespace Application.MediatR.Queries.Order;

public static class GetOrdersByGridifyQuery
{
    public sealed record Query(GridifyQuery GridifyQuery) : IQuery<Paging<IOrder>>;

    internal sealed class Handler : IRequestHandler<Query, Result<Paging<IOrder>>>
    {
        public Task<Result<Paging<IOrder>>> Handle(Query request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
using Application.Models.Order;
using MediatR;
using Remora.Results;

namespace Application.MediatR.Queries.Order;

public static class GetOrderById
{
    public sealed record Query(Guid Id) : IQuery<IOrder>;

    internal sealed class Handler : IRequestHandler<Query, Result<IOrder>>
    {
        public Task<Result<IOrder>> Handle(Query request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
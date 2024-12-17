using MediatR;
using Remora.Results;

namespace Application.MediatR.Behaviors;

public class ExceptionToResultBehavior<TRequest,TResult> : IPipelineBehavior<TRequest,TResult> where TRequest : IRequest<TResult>
{
    public async Task<TResult> Handle(TRequest request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            if (typeof(TResult).IsAssignableTo(typeof(IResult)))
            {
                return (TResult)(object)Result.FromError(new ExceptionError(ex));
            }

            throw;
        }
    }
}
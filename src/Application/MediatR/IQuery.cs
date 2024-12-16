using MediatR;
using Remora.Results;

namespace Application.MediatR;

public interface IQuery<TResult> : IRequest<Result<TResult>>
{
    
}
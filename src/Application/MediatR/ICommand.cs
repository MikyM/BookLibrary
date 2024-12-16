using MediatR;
using Remora.Results;

namespace Application.MediatR;

public interface ICommand<TResult> : IRequest<Result<TResult>>
{
    
}

public interface ICommand : IRequest<Result>
{
    
}
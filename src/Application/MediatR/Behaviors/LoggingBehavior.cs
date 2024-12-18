using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.MediatR.Behaviors;

public class LoggingBehavior<TRequest, TResult>(ILogger<LoggingBehavior<TRequest, TResult>> logger)
    : IPipelineBehavior<TRequest, TResult> where TRequest : IRequest<TResult>
{
    public async Task<TResult> Handle(TRequest request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken)
    {
        // make log level dynamic
        var st = new Stopwatch();

        // extract this higher
        var id = Guid.NewGuid().ToString();
        
        logger.LogDebug("Handling request {@Request} with ID: {Id}", request, id);
        
        st.Restart();

        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Handling request {Id} results in an exception", id);

            throw;
        }
        finally
        {
            st.Stop();
            
            logger.LogDebug("Handled request {Id} took {@Duration}ms", id, st.ElapsedMilliseconds);
        }
    }
}
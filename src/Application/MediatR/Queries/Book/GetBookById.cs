
using DataAccess;
using DataAccess.Specifications.Book;
using DataExplorer.EfCore.Abstractions.DataServices;
using MediatR;
using Models.Book;
using Remora.Results;

namespace Application.MediatR.Queries.Book;

public static class GetBookById
{
    public sealed record Query(long Id) : IQuery<IBookPayload>;

    [UsedImplicitly]
    public sealed class Handler : IRequestHandler<Query, Result<IBookPayload>>
    {
        private readonly IReadOnlyDataService<Domain.Book,IBookLibraryDbContext> _dataService;

        public Handler(IReadOnlyDataService<Domain.Book, IBookLibraryDbContext> dataService)
        {
            _dataService = dataService;
        }

        public async Task<Result<IBookPayload>> Handle(Query request, CancellationToken cancellationToken)
        {
            var spec = new EntityProjectedSpecification<Domain.Book,long,BookPayload>(request.Id);
            
            var getResult = await _dataService.GetSingleAsync(spec, cancellationToken);

            return !getResult.IsDefined(out var entity) 
                ? Result<IBookPayload>.FromError(getResult) 
                : entity;
        }
    }
}
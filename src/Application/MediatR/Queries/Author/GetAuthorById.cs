using DataAccess;
using DataAccess.Specifications.Book;
using DataExplorer.EfCore.Abstractions.DataServices;
using MediatR;
using Models.Author;
using Remora.Results;

namespace Application.MediatR.Queries.Order;

public static class GetAuthorById
{
    public sealed record Query(long Id) : IQuery<IAuthorPayload>;

    public sealed class Handler : IRequestHandler<Query, Result<IAuthorPayload>>
    {
        private readonly IReadOnlyDataService<Domain.Author, IBookLibraryDbContext> _dataService;

        public Handler(IReadOnlyDataService<Domain.Author, IBookLibraryDbContext> dataService)
        {
            _dataService = dataService;
        }

        public async Task<Result<IAuthorPayload>> Handle(Query request, CancellationToken cancellationToken)
        {
            var spec = new EntityProjectedSpecification<Author,long,AuthorPayload>(request.Id);
            
            var getResult = await _dataService.GetSingleAsync(spec, cancellationToken);

            return !getResult.IsDefined(out var entity) 
                ? Result<IAuthorPayload>.FromError(getResult) 
                : entity;
        }
    }
}
using DataAccess;
using DataExplorer.EfCore.Abstractions.DataServices;
using MediatR;
using Models.Book;
using OneOf;
using Remora.Results;

namespace Application.MediatR.Commands.Book;

public static class AddBook
{
    public sealed record Command(IAddBookPayload Payload, bool ShouldReturnCreated) : ICommand<OneOf<Unit, IBookPayload>>;

    public sealed class Handler : IRequestHandler<Command, Result<OneOf<Unit, IBookPayload>>>
    {
        private readonly ICrudDataService<Domain.Book, IBookLibraryDbContext> _dataService;

        public Handler(ICrudDataService<Domain.Book, IBookLibraryDbContext> dataService)
        {
            _dataService = dataService;
        }

        public async Task<Result<OneOf<Unit, IBookPayload>>> Handle(Command request, CancellationToken cancellationToken)
        {
            var mapped = _dataService.Mapper.Map<Domain.Book>(request.Payload);
            
            var addRes = await _dataService.AddAsync(mapped, true, cancellationToken);

            if (!addRes.IsSuccess)
            {
                return Result<OneOf<Unit, IBookPayload>>.FromError(addRes);
            }

            if (request.ShouldReturnCreated)
            {
                return OneOf<Unit, IBookPayload>.FromT1(_dataService.Mapper.Map<IBookPayload>(mapped));
            }
            
            return OneOf<Unit, IBookPayload>.FromT0(Unit.Value);
        }
    }
}
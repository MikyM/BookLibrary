using Application.Models.Book;
using MediatR;
using OneOf;
using Remora.Results;

namespace Application.MediatR.Commands.Book;

public static class AddBook
{
    public sealed record Command(IAddBookPayload Payload) : ICommand<OneOf<Unit, IBook>>;

    internal sealed class Handler : IRequestHandler<Command, Result<OneOf<Unit, IBook>>>
    {
        public Task<Result<OneOf<Unit, IBook>>> Handle(Command request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
using Application.Models.Book;
using FluentValidation;

namespace Application.Validators;

public class AddBookPayloadValidator : AbstractValidator<IAddBookPayload>
{
    public AddBookPayloadValidator()
    {
        // TODO
    }
}
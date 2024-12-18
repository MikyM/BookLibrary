using FluentValidation;
using Models.Book;

namespace Application.Validators;

[UsedImplicitly]
public class AddBookPayloadValidator : AbstractValidator<IAddBookPayload>
{
    public AddBookPayloadValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Continue;

        RuleFor(x => x.Bookstand)
            .NotEmpty();

        RuleFor(x => x.Shelf)
            .NotEmpty();

        RuleFor(x => x.Price)
            .NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty();

        RuleFor(x => x.Authors)
            .NotEmpty();

        RuleForEach(x => x.Authors)
            .ChildRules(y =>
            {
                y.RuleFor(x => x.FirstName)
                    .NotEmpty();

                y.RuleFor(x => x.LastName)
                    .NotEmpty();
            });
    }
}
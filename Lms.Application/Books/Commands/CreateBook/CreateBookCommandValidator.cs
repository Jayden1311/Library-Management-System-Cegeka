using FluentValidation;

namespace Lms.Application.Books.Commands.CreateBook;

public class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
{
    public CreateBookCommandValidator()
    {
        RuleFor(x => x.LibraryId).GreaterThan(0).WithMessage("Library ID must be greater than 0.");
        RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required.")
            .MaximumLength(100).WithMessage("Title must not exceed 100 characters.");
        RuleFor(x => x.Author).NotEmpty().WithMessage("Author is required.")
            .MaximumLength(100).WithMessage("Author must not exceed 100 characters.");
        RuleFor(x => x.Genre).NotEmpty().WithMessage("Genre is required.")
            .MaximumLength(50).WithMessage("Genre must not exceed 50 characters.");
        RuleFor(x => x.ISBN).NotEmpty().WithMessage("ISBN is required.")
            .MaximumLength(13).WithMessage("ISBN must not exceed 13 characters.");
    }
}
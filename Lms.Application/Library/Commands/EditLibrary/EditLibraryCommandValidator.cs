using FluentValidation;

namespace Lms.Application.Library.Commands.EditLibrary;

public class EditLibraryCommandValidator : AbstractValidator<EditLibraryCommand>
{
    public EditLibraryCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id must be greater than 0.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");
    }
}
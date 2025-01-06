using FluentValidation;

namespace Lms.Application.Patron.Commands.CreatePatronCommand;

public class CreatePatronCommandValidator : AbstractValidator<CreatePatronCommand>
{
    public CreatePatronCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");
    }
}
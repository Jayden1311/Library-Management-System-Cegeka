using FluentValidation;

namespace Lms.Application.Patron.Commands.DeletePatron;

public class DeletePatronCommandValidator : AbstractValidator<DeletePatronCommand>
{
    public DeletePatronCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id must be greater than 0.");
    }
}
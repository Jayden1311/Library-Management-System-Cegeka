using FluentValidation;
using Lms.Domain.Interfaces.Repositories;
using MediatR;

namespace Lms.Application.Patron.Commands;

public sealed record EditPatronCommand(int Id, string Name) : IRequest;

public sealed class EditPatronCommandHandler : IRequestHandler<EditPatronCommand>
{
    private readonly IPatronRepository _patronRepository;
    private readonly IValidator<EditPatronCommand> _validator;

    public EditPatronCommandHandler(IPatronRepository patronRepository, IValidator<EditPatronCommand> validator)
    {
        _patronRepository = patronRepository;
        _validator = validator;
    }

    public async Task Handle(EditPatronCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var patron = await _patronRepository.GetByIdAsync(request.Id);
        if (patron == null)
        {
            throw new KeyNotFoundException($"Patron with ID {request.Id} not found.");
        }

        patron.UpdateName(request.Name);
        await _patronRepository.EditAsync(patron);
        await _patronRepository.SaveChangesAsync();
    }
}
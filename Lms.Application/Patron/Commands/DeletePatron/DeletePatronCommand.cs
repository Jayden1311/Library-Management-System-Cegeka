using FluentValidation;
using Lms.Domain.Interfaces.Repositories;
using MediatR;

namespace Lms.Application.Patron.Commands.DeletePatron;

public sealed record DeletePatronCommand(int Id) : IRequest;

public sealed class DeletePatronCommandHandler : IRequestHandler<DeletePatronCommand>
{
    private readonly IPatronRepository _patronRepository;
    private readonly IValidator<DeletePatronCommand> _validator;

    public DeletePatronCommandHandler(IPatronRepository patronRepository, IValidator<DeletePatronCommand> validator)
    {
        _patronRepository = patronRepository;
        _validator = validator;
    }

    public async Task Handle(DeletePatronCommand request, CancellationToken cancellationToken)
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

        await _patronRepository.DeleteAsync(patron);
        await _patronRepository.SaveChangesAsync();
    }
}
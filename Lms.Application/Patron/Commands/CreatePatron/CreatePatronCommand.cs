using FluentValidation;
using Lms.Domain.Interfaces.Repositories;
using MediatR;

namespace Lms.Application.Patron.Commands.CreatePatronCommand;

public sealed record CreatePatronCommand(string Name) : IRequest<int>;

public sealed class CreatePatronCommandHandler : IRequestHandler<CreatePatronCommand, int>
{
    private readonly IPatronRepository _patronRepository;
    private readonly IValidator<CreatePatronCommand> _validator;

    public CreatePatronCommandHandler(IPatronRepository patronRepository, IValidator<CreatePatronCommand> validator)
    {
        _patronRepository = patronRepository;
        _validator = validator;
    }

    public async Task<int> Handle(CreatePatronCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var patron = new Domain.Entities.Patron(request.Name);
        await _patronRepository.AddAsync(patron);
        await _patronRepository.SaveChangesAsync();
        return patron.Id;
    }
}
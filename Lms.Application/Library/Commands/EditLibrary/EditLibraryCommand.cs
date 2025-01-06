using FluentValidation;
using Lms.Domain.Interfaces;
using Lms.Domain.Interfaces.Repositories;
using MediatR;

namespace Lms.Application.Library.Commands.EditLibrary;

public sealed record EditLibraryCommand(int Id, string Name) : IRequest;

public sealed class EditLibraryCommandHandler : IRequestHandler<EditLibraryCommand>
{
    private readonly ILibraryRepository _libraryRepository;
    private readonly IValidator<EditLibraryCommand> _validator;

    public EditLibraryCommandHandler(ILibraryRepository libraryRepository, IValidator<EditLibraryCommand> validator)
    {
        _libraryRepository = libraryRepository;
        _validator = validator;
    }

    public async Task Handle(EditLibraryCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var library = await _libraryRepository.GetByIdAsync(request.Id);
        if (library == null)
        {
            throw new KeyNotFoundException($"Library with ID {request.Id} not found.");
        }

        library.UpdateName(request.Name);
        await _libraryRepository.EditAsync(library);
        await _libraryRepository.SaveChangesAsync();
    }
}
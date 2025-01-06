using FluentValidation;
using Lms.Domain.Interfaces;
using Lms.Domain.Interfaces.Repositories;
using MediatR;

namespace Lms.Application.Library.Commands;

public sealed record DeleteLibraryCommand(int Id) : IRequest;

public sealed class DeleteLibraryCommandHandler : IRequestHandler<DeleteLibraryCommand>
{
    private readonly ILibraryRepository _libraryRepository;
    private readonly IValidator<DeleteLibraryCommand> _validator;

    public DeleteLibraryCommandHandler(ILibraryRepository libraryRepository, IValidator<DeleteLibraryCommand> validator)
    {
        _libraryRepository = libraryRepository;
        _validator = validator;
    }

    public async Task Handle(DeleteLibraryCommand request, CancellationToken cancellationToken)
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

        await _libraryRepository.DeleteAsync(library);
        await _libraryRepository.SaveChangesAsync();
    }
}
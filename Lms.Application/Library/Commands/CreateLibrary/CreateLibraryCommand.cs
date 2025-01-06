using FluentValidation;
using Lms.Domain.Interfaces;
using Lms.Domain.Interfaces.Repositories;
using MediatR;

namespace Lms.Application.Library.Commands.CreateLibrary;

public sealed record CreateLibraryCommand(string Name) : IRequest<int>;

public sealed class CreateLibraryCommandHandler : IRequestHandler<CreateLibraryCommand, int>
{
    private readonly ILibraryRepository _libraryRepository;
    private readonly IValidator<CreateLibraryCommand> _validator;

    public CreateLibraryCommandHandler(ILibraryRepository libraryRepository, IValidator<CreateLibraryCommand> validator)
    {
        _libraryRepository = libraryRepository;
        _validator = validator;
    }

    public async Task<int> Handle(CreateLibraryCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var library = new Domain.Aggregates.Library(request.Name);
        await _libraryRepository.AddAsync(library);
        await _libraryRepository.SaveChangesAsync();

        return library.Id;
    }
}
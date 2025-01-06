using FluentValidation;
using Lms.Domain.Entities;
using Lms.Domain.Interfaces.Repositories;
using MediatR;

namespace Lms.Application.Books.Commands.CreateBook;

public sealed record CreateBookCommand(int LibraryId, string Title, string Author, string Genre, string ISBN)
    : IRequest<int>;

public sealed class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, int>
{
    private readonly ILibraryRepository _libraryRepository;
    private readonly IValidator<CreateBookCommand> _validator;

    public CreateBookCommandHandler(ILibraryRepository libraryRepository, IValidator<CreateBookCommand> validator)
    {
        _libraryRepository = libraryRepository;
        _validator = validator;
    }

    public async Task<int> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var library = await _libraryRepository.GetByIdAsync(request.LibraryId);
        if (library == null)
        {
            throw new KeyNotFoundException($"Library with ID {request.LibraryId} not found.");
        }

        var book = new Book(request.Title, request.Author, request.Genre, request.ISBN, library);
        library.AddBook(book);

        await _libraryRepository.SaveChangesAsync();
        return book.Id;
    }
}
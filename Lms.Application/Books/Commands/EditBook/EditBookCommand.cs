using FluentValidation;
using Lms.Domain.Interfaces;
using Lms.Domain.Interfaces.Repositories;
using MediatR;

namespace Lms.Application.Books.Commands.EditBook;

public sealed record EditBookCommand(int LibraryId, int BookId, string Title, string Author, string Genre, string ISBN)
    : IRequest;

public sealed class EditBookCommandHandler : IRequestHandler<EditBookCommand>
{
    private readonly ILibraryRepository _libraryRepository;
    private readonly IValidator<EditBookCommand> _validator;

    public EditBookCommandHandler(ILibraryRepository libraryRepository, IValidator<EditBookCommand> validator)
    {
        _libraryRepository = libraryRepository;
        _validator = validator;
    }

    public async Task Handle(EditBookCommand request, CancellationToken cancellationToken)
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

        var book = library.Books.FirstOrDefault(b => b.Id == request.BookId);
        if (book == null)
        {
            throw new KeyNotFoundException($"Book with ID {request.BookId} not found.");
        }

        book.UpdateProperties(request.Title, request.Author, request.Genre);
        book.UpdateISBN(request.ISBN);

        await _libraryRepository.SaveChangesAsync();
    }
}
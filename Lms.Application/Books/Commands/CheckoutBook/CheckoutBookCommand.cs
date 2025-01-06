using FluentValidation;
using Lms.Domain.Interfaces.Repositories;
using MediatR;

namespace Lms.Application.Books.Commands.CheckoutBook;

public sealed record CheckoutBookCommand(int PatronId, int LibraryID, string ISBN) : IRequest;

public sealed class CheckoutBookCommandHandler : IRequestHandler<CheckoutBookCommand>
{
    private readonly ILibraryRepository _libraryRepository;
    private readonly IPatronRepository _patronRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IValidator<CheckoutBookCommand> _validator;

    public CheckoutBookCommandHandler(ILibraryRepository libraryRepository, IPatronRepository patronRepository,
        IBookRepository bookRepository, IValidator<CheckoutBookCommand> validator)
    {
        _libraryRepository = libraryRepository;
        _patronRepository = patronRepository;
        _bookRepository = bookRepository;
        _validator = validator;
    }

    public async Task Handle(CheckoutBookCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var library = await _libraryRepository.GetByIdAsync(request.LibraryID);
        if (library == null)
        {
            throw new KeyNotFoundException($"Library with ID {request.LibraryID} not found.");
        }

        var patron = await _patronRepository.GetByIdAsync(request.PatronId);
        if (patron == null)
        {
            throw new KeyNotFoundException($"Patron with ID {request.PatronId} not found.");
        }

        var book = await _bookRepository.GetByISBNAsync(request.ISBN);
        if (book == null || book.LibraryId != request.LibraryID || !book.IsAvailable)
        {
            throw new InvalidOperationException("Book is not available at the specified library.");
        }

        library.CheckoutBook(book.ISBN, patron);
        await _libraryRepository.SaveChangesAsync();
    }
}
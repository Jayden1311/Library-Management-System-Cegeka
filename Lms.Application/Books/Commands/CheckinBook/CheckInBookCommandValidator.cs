using FluentValidation;
using Lms.Domain.Interfaces.Repositories;

namespace Lms.Application.Books.Commands.CheckinBook;

public sealed class CheckinBookCommandValidator : AbstractValidator<CheckinBookCommand>
{
    private readonly IPatronRepository _patronRepository;
    private readonly IBookRepository _bookRepository;
    private readonly ILibraryRepository _libraryRepository;

    public CheckinBookCommandValidator(IPatronRepository patronRepository, IBookRepository bookRepository,
        ILibraryRepository libraryRepository)
    {
        _patronRepository = patronRepository;
        _bookRepository = bookRepository;
        _libraryRepository = libraryRepository;

        RuleFor(x => x.PatronId)
            .GreaterThan(0)
            .MustAsync(BeAnExistingPatron).WithMessage("Patron does not exist.");

        RuleFor(x => x.ISBN)
            .NotEmpty()
            .MustAsync(BeAnExistingBook).WithMessage("Book does not exist.")
            .MustAsync(BeCheckedOutBook).WithMessage("Book is not checked out from the specified library.");
    }

    private async Task<bool> BeAnExistingPatron(int patronId, CancellationToken cancellationToken)
    {
        var patron = await _patronRepository.GetByIdAsync(patronId);
        return patron != null;
    }

    private async Task<bool> BeAnExistingBook(string isbn, CancellationToken cancellationToken)
    {
        return await _bookRepository.GetByISBNAsync(isbn) != null;
    }

    private async Task<bool> BeCheckedOutBook(CheckinBookCommand command, string isbn,
        CancellationToken cancellationToken)
    {
        var library = await _libraryRepository.GetByIdAsync(command.LibraryID);
        if (library == null)
        {
            return false;
        }

        var book = library.Books.FirstOrDefault(b => b.ISBN == isbn);
        return book != null && !book.IsAvailable;
    }
}
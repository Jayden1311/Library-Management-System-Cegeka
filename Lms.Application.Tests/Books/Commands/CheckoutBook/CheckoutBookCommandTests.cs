using FluentValidation;
using FluentValidation.Results;
using Lms.Application.Books.Commands.CheckoutBook;
using Lms.Domain.Entities;
using Lms.Domain.Interfaces.Repositories;
using MediatR;
using Moq;
using Xunit;

namespace Lms.Application.Tests.Books.Commands.CheckoutBook;

public class CheckoutBookCommandHandlerTests
{
    private readonly Mock<ILibraryRepository> _libraryRepositoryMock;
    private readonly Mock<IPatronRepository> _patronRepositoryMock;
    private readonly Mock<IBookRepository> _bookRepositoryMock;
    private readonly Mock<IValidator<CheckoutBookCommand>> _validatorMock;
    private readonly IRequestHandler<CheckoutBookCommand> _handler;

    public CheckoutBookCommandHandlerTests()
    {
        _libraryRepositoryMock = new Mock<ILibraryRepository>();
        _patronRepositoryMock = new Mock<IPatronRepository>();
        _bookRepositoryMock = new Mock<IBookRepository>();
        _validatorMock = new Mock<IValidator<CheckoutBookCommand>>();
        _handler = new CheckoutBookCommandHandler(_libraryRepositoryMock.Object, _patronRepositoryMock.Object,
            _bookRepositoryMock.Object, _validatorMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Throw_Exception_When_Validation_Fails()
    {
        var command = new CheckoutBookCommand(1, 1, "1234567890");
        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(
            new ValidationResult(new List<ValidationFailure>
                { new ValidationFailure("PatronId", "Patron does not exist.") }));

        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_Throw_Exception_When_Library_Not_Found()
    {
        var command = new CheckoutBookCommand(1, 1, "1234567890");
        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _libraryRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Domain.Aggregates.Library?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_Checkout_Book_When_Valid()
    {
        var library = new Domain.Aggregates.Library("Test Library");
        var patron = new Domain.Entities.Patron("Test Patron");
        var book = new Book("Test Title", "Test Author", "Test Genre", "1234567890", library);
        library.AddBook(book);

        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<CheckoutBookCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _libraryRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(library);
        _patronRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(patron);
        _bookRepositoryMock.Setup(repo => repo.GetByISBNAsync(It.IsAny<string>())).ReturnsAsync(book);

        var command = new CheckoutBookCommand(patron.Id, library.Id, "1234567890");
        await _handler.Handle(command, CancellationToken.None);

        Assert.False(book.IsAvailable);
        Assert.Contains(book, patron.CheckedOutBooks);
    }
}
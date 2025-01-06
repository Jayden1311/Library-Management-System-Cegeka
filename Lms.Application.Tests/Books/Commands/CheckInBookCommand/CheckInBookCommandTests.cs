using FluentValidation;
using FluentValidation.Results;
using Lms.Application.Books.Commands.CheckinBook;
using Lms.Domain.Entities;
using Lms.Domain.Interfaces.Repositories;
using MediatR;
using Moq;
using Xunit;

namespace Lms.Application.Tests.Books.Commands.CheckinBook;

public class CheckinBookCommandHandlerTests
{
    private readonly Mock<ILibraryRepository> _libraryRepositoryMock;
    private readonly Mock<IPatronRepository> _patronRepositoryMock;
    private readonly Mock<IBookRepository> _bookRepositoryMock;
    private readonly Mock<IValidator<CheckinBookCommand>> _validatorMock;
    private readonly IRequestHandler<CheckinBookCommand> _handler;

    public CheckinBookCommandHandlerTests()
    {
        _libraryRepositoryMock = new Mock<ILibraryRepository>();
        _patronRepositoryMock = new Mock<IPatronRepository>();
        _bookRepositoryMock = new Mock<IBookRepository>();
        _validatorMock = new Mock<IValidator<CheckinBookCommand>>();
        _handler = new CheckinBookCommandHandler(_libraryRepositoryMock.Object, _patronRepositoryMock.Object,
            _bookRepositoryMock.Object, _validatorMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Throw_Exception_When_Validation_Fails()
    {
        var command = new CheckinBookCommand(1, 1, "1234567890");
        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(
            new ValidationResult(new List<ValidationFailure>
                { new ValidationFailure("PatronId", "Patron does not exist.") }));

        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_Throw_Exception_When_Library_Not_Found()
    {
        var command = new CheckinBookCommand(1, 1, "1234567890");
        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _libraryRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Domain.Aggregates.Library?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_Checkin_Book_When_Valid()
    {
        var library = new Domain.Aggregates.Library("Test Library");
        var patron = new Domain.Entities.Patron("Test Patron");
        var book = new Book("Test Title", "Test Author", "Test Genre", "1234567890", library);
        library.AddBook(book);
        library.CheckoutBook(book.ISBN, patron);

        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<CheckinBookCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _libraryRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(library);
        _patronRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(patron);
        _bookRepositoryMock.Setup(repo => repo.GetByISBNAsync(It.IsAny<string>())).ReturnsAsync(book);

        var command = new CheckinBookCommand(patron.Id, library.Id, "1234567890");
        await _handler.Handle(command, CancellationToken.None);

        Assert.True(book.IsAvailable);
        Assert.DoesNotContain(book, patron.CheckedOutBooks);
    }
}
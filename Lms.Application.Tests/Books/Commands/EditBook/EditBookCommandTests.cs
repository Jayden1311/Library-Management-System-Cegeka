using FluentValidation;
using FluentValidation.Results;
using Lms.Application.Books.Commands.EditBook;
using Lms.Domain.Entities;
using Lms.Domain.Interfaces.Repositories;
using Moq;
using Xunit;

namespace Lms.Application.Tests.Books.Commands.EditBook;

public class EditBookCommandHandlerTests
{
    private readonly Mock<ILibraryRepository> _libraryRepositoryMock;
    private readonly Mock<IValidator<EditBookCommand>> _validatorMock;
    private readonly EditBookCommandHandler _handler;

    public EditBookCommandHandlerTests()
    {
        _libraryRepositoryMock = new Mock<ILibraryRepository>();
        _validatorMock = new Mock<IValidator<EditBookCommand>>();
        _handler = new EditBookCommandHandler(_libraryRepositoryMock.Object, _validatorMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldEditBook_WhenCommandIsValid()
    {
        // Arrange
        var command = new EditBookCommand(1, 0, "New Title", "New Author", "New Genre", "1234567890123");
        var library = new Domain.Aggregates.Library("Test Library");
        var book = new Book("Title", "Author", "Genre", "1234567890123", library);
        library.AddBook(book); // Ensure the book is added to the library
        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _libraryRepositoryMock.Setup(r => r.GetByIdAsync(command.LibraryId)).ReturnsAsync(library);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _libraryRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        Assert.Equal("New Title", book.Title);
        Assert.Equal("New Author", book.Author);
        Assert.Equal("New Genre", book.Genre);
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenCommandIsInvalid()
    {
        // Arrange
        var command = new EditBookCommand(1, 1, "", "", "", "");
        var validationResult = new ValidationResult(new List<ValidationFailure>
            { new ValidationFailure("Title", "Title is required") });
        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
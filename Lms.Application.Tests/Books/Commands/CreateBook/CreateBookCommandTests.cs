using FluentValidation;
using FluentValidation.Results;
using Lms.Application.Books.Commands.CreateBook;
using Lms.Domain.Interfaces.Repositories;
using Moq;
using Xunit;

namespace Lms.Application.Tests.Books.Commands.CreateBook;

public class CreateBookCommandHandlerTests
{
    private readonly Mock<ILibraryRepository> _libraryRepositoryMock;
    private readonly Mock<IValidator<CreateBookCommand>> _validatorMock;
    private readonly CreateBookCommandHandler _handler;

    public CreateBookCommandHandlerTests()
    {
        _libraryRepositoryMock = new Mock<ILibraryRepository>();
        _validatorMock = new Mock<IValidator<CreateBookCommand>>();
        _handler = new CreateBookCommandHandler(_libraryRepositoryMock.Object, _validatorMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateBook_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreateBookCommand(1, "Title", "Author", "Genre", "1234567890123");
        var library = new Domain.Aggregates.Library("Test Library");
        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
        _libraryRepositoryMock.Setup(r => r.GetByIdAsync(command.LibraryId)).ReturnsAsync(library);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _libraryRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        Assert.Equal(library.Books.First().Id, result);
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenCommandIsInvalid()
    {
        // Arrange
        var command = new CreateBookCommand(1, "", "", "", "");
        var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("Title", "Title is required") });
        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(validationResult);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
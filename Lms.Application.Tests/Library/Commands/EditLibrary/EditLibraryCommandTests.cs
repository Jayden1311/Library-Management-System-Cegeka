using FluentValidation;
using Lms.Application.Library.Commands.EditLibrary;
using Lms.Domain.Interfaces;
using Lms.Domain.Interfaces.Repositories;
using Moq;
using Xunit;

namespace Lms.Application.Tests.Library.Commands.EditLibrary;

public class EditLibraryCommandTests
{
    private readonly Mock<ILibraryRepository> _libraryRepositoryMock;
    private readonly Mock<IValidator<EditLibraryCommand>> _validatorMock;
    private readonly EditLibraryCommandHandler _handler;

    public EditLibraryCommandTests()
    {
        _libraryRepositoryMock = new Mock<ILibraryRepository>();
        _validatorMock = new Mock<IValidator<EditLibraryCommand>>();
        _handler = new EditLibraryCommandHandler(_libraryRepositoryMock.Object, _validatorMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldEditLibrary_WhenCommandIsValid()
    {
        // Arrange
        var command = new EditLibraryCommand(1, "Updated Library");
        var library = new Domain.Aggregates.Library("Old Library");
        _validatorMock.Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _libraryRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(library);
        _libraryRepositoryMock.Setup(r => r.EditAsync(library)).Returns(Task.CompletedTask);
        _libraryRepositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, default);

        // Assert
        _libraryRepositoryMock.Verify(r => r.EditAsync(library), Times.Once);
        _libraryRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        Assert.Equal("Updated Library", library.Name);
    }

    [Fact]
    public async Task Handle_ShouldThrowKeyNotFoundException_WhenLibraryDoesNotExist()
    {
        // Arrange
        var command = new EditLibraryCommand(1, "Updated Library");
        _validatorMock.Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _libraryRepositoryMock.Setup(r => r.GetByIdAsync(1))!.ReturnsAsync((Domain.Aggregates.Library)null!);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, default));
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenCommandIsInvalid()
    {
        // Arrange
        var command = new EditLibraryCommand(1, string.Empty);
        _validatorMock.Setup(v => v.ValidateAsync(command, default)).ReturnsAsync(
            new FluentValidation.Results.ValidationResult(new[]
                { new FluentValidation.Results.ValidationFailure("Name", "Name is required.") }));

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, default));
    }
}
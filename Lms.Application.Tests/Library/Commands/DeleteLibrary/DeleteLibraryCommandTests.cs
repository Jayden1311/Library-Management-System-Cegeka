using FluentValidation;
using Lms.Application.Library.Commands;
using Lms.Domain.Interfaces;
using Lms.Domain.Interfaces.Repositories;
using Moq;
using Xunit;

namespace Lms.Application.Tests.Library.Commands.DeleteLibrary;

public class DeleteLibraryCommandTests
{
    private readonly Mock<ILibraryRepository> _libraryRepositoryMock;
    private readonly Mock<IValidator<DeleteLibraryCommand>> _validatorMock;
    private readonly DeleteLibraryCommandHandler _handler;

    public DeleteLibraryCommandTests()
    {
        _libraryRepositoryMock = new Mock<ILibraryRepository>();
        _validatorMock = new Mock<IValidator<DeleteLibraryCommand>>();
        _handler = new DeleteLibraryCommandHandler(_libraryRepositoryMock.Object, _validatorMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldDeleteLibrary_WhenCommandIsValid()
    {
        // Arrange
        var command = new DeleteLibraryCommand(1);
        var library = new Domain.Aggregates.Library("Library to Delete");
        _validatorMock.Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _libraryRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(library);
        _libraryRepositoryMock.Setup(r => r.DeleteAsync(library)).Returns(Task.CompletedTask);
        _libraryRepositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, default);

        // Assert
        _libraryRepositoryMock.Verify(r => r.DeleteAsync(library), Times.Once);
        _libraryRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowKeyNotFoundException_WhenLibraryDoesNotExist()
    {
        // Arrange
        var command = new DeleteLibraryCommand(1);
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
        var command = new DeleteLibraryCommand(0);
        _validatorMock.Setup(v => v.ValidateAsync(command, default)).ReturnsAsync(
            new FluentValidation.Results.ValidationResult(new[]
                { new FluentValidation.Results.ValidationFailure("Id", "Id must be greater than 0.") }));

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, default));
    }
}
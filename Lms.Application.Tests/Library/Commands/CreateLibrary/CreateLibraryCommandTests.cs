using FluentValidation;
using Lms.Application.Library.Commands.CreateLibrary;
using Lms.Domain.Interfaces;
using Lms.Domain.Interfaces.Repositories;
using Moq;
using Xunit;

namespace Lms.Application.Tests.Library.Commands.CreateLibrary;

public class CreateLibraryCommandTests
{
    private readonly Mock<ILibraryRepository> _libraryRepositoryMock;
    private readonly Mock<IValidator<CreateLibraryCommand>> _validatorMock;
    private readonly CreateLibraryCommandHandler _handler;

    public CreateLibraryCommandTests()
    {
        _libraryRepositoryMock = new Mock<ILibraryRepository>();
        _validatorMock = new Mock<IValidator<CreateLibraryCommand>>();
        _handler = new CreateLibraryCommandHandler(_libraryRepositoryMock.Object, _validatorMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateLibrary_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreateLibraryCommand("New Library");
        _validatorMock.Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _libraryRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Domain.Aggregates.Library>()))
            .Callback<Domain.Aggregates.Library>(library => library.GetType().GetProperty("Id")!.SetValue(library, 1))
            .Returns(Task.CompletedTask);
        _libraryRepositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        Assert.NotEqual(0, result);
        _libraryRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Domain.Aggregates.Library>()), Times.Once);
        _libraryRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenCommandIsInvalid()
    {
        // Arrange
        var command = new CreateLibraryCommand(string.Empty);
        _validatorMock.Setup(v => v.ValidateAsync(command, default)).ReturnsAsync(
            new FluentValidation.Results.ValidationResult(new[]
                { new FluentValidation.Results.ValidationFailure("Name", "Name is required.") }));

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, default));
    }
}
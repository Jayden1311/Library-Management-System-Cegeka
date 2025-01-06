using FluentValidation;
using FluentValidation.Results;
using Lms.Application.Patron.Commands;
using Lms.Domain.Interfaces.Repositories;
using Moq;
using Xunit;

namespace Lms.Application.Tests.Patron.Commands.EditPatron;

public class EditPatronCommandHandlerTests
{
    private readonly Mock<IPatronRepository> _patronRepositoryMock;
    private readonly Mock<IValidator<EditPatronCommand>> _validatorMock;
    private readonly EditPatronCommandHandler _handler;

    public EditPatronCommandHandlerTests()
    {
        _patronRepositoryMock = new Mock<IPatronRepository>();
        _validatorMock = new Mock<IValidator<EditPatronCommand>>();
        _handler = new EditPatronCommandHandler(_patronRepositoryMock.Object, _validatorMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldEditPatron_WhenCommandIsValid()
    {
        // Arrange
        var command = new EditPatronCommand(1, "John Doe");
        var patron = new Domain.Entities.Patron("Jane Doe");
        _validatorMock.Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult());
        _patronRepositoryMock.Setup(r => r.GetByIdAsync(command.Id))
            .ReturnsAsync(patron);

        // Act
        await _handler.Handle(command, default);

        // Assert
        _patronRepositoryMock.Verify(r => r.EditAsync(patron), Times.Once);
        _patronRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenCommandIsInvalid()
    {
        // Arrange
        var command = new EditPatronCommand(1, "John Doe");
        var validationFailures = new List<ValidationFailure> { new ValidationFailure("Name", "Name is required.") };
        _validatorMock.Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult(validationFailures));

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, default));
    }

    [Fact]
    public async Task Handle_ShouldThrowKeyNotFoundException_WhenPatronNotFound()
    {
        // Arrange
        var command = new EditPatronCommand(1, "John Doe");
        _validatorMock.Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult());
        _patronRepositoryMock.Setup(r => r.GetByIdAsync(command.Id))
            .ReturnsAsync((Domain.Entities.Patron)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, default));
    }
}
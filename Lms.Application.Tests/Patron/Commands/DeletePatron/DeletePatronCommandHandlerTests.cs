using FluentValidation;
using FluentValidation.Results;
using Lms.Application.Patron.Commands.DeletePatron;
using Lms.Domain.Interfaces.Repositories;
using Moq;
using Xunit;

namespace Lms.Application.Tests.Patron.Commands.DeletePatron;

public class DeletePatronCommandHandlerTests
{
    private readonly Mock<IPatronRepository> _patronRepositoryMock;
    private readonly Mock<IValidator<DeletePatronCommand>> _validatorMock;
    private readonly DeletePatronCommandHandler _handler;

    public DeletePatronCommandHandlerTests()
    {
        _patronRepositoryMock = new Mock<IPatronRepository>();
        _validatorMock = new Mock<IValidator<DeletePatronCommand>>();
        _handler = new DeletePatronCommandHandler(_patronRepositoryMock.Object, _validatorMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldDeletePatron_WhenCommandIsValid()
    {
        // Arrange
        var command = new DeletePatronCommand(1);
        var patron = new Domain.Entities.Patron("John Doe");
        _validatorMock.Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult());
        _patronRepositoryMock.Setup(r => r.GetByIdAsync(command.Id))
            .ReturnsAsync(patron);

        // Act
        await _handler.Handle(command, default);

        // Assert
        _patronRepositoryMock.Verify(r => r.DeleteAsync(patron), Times.Once);
        _patronRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenCommandIsInvalid()
    {
        // Arrange
        var command = new DeletePatronCommand(1);
        var validationFailures = new List<ValidationFailure>
            { new ValidationFailure("Id", "Id must be greater than 0.") };
        _validatorMock.Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult(validationFailures));

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, default));
    }

    [Fact]
    public async Task Handle_ShouldThrowKeyNotFoundException_WhenPatronNotFound()
    {
        // Arrange
        var command = new DeletePatronCommand(1);
        _validatorMock.Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult());
        _patronRepositoryMock.Setup(r => r.GetByIdAsync(command.Id))
            .ReturnsAsync((Domain.Entities.Patron)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, default));
    }
}
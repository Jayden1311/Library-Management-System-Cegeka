using FluentValidation;
using FluentValidation.Results;
using Lms.Application.Patron.Commands.CreatePatronCommand;
using Lms.Domain.Interfaces.Repositories;
using Moq;
using Xunit;

namespace Lms.Application.Tests.Patron.Commands.CreatePatron;

public class CreatePatronCommandHandlerTests
{
    private readonly Mock<IPatronRepository> _patronRepositoryMock;
    private readonly Mock<IValidator<CreatePatronCommand>> _validatorMock;
    private readonly CreatePatronCommandHandler _handler;

    public CreatePatronCommandHandlerTests()
    {
        _patronRepositoryMock = new Mock<IPatronRepository>();
        _validatorMock = new Mock<IValidator<CreatePatronCommand>>();
        _handler = new CreatePatronCommandHandler(_patronRepositoryMock.Object, _validatorMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreatePatron_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreatePatronCommand("John Doe");
        _validatorMock.Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult());

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        _patronRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.Patron>()), Times.Once);
        _patronRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenCommandIsInvalid()
    {
        // Arrange
        var command = new CreatePatronCommand("John Doe");
        var validationFailures = new List<ValidationFailure> { new ValidationFailure("Name", "Name is required.") };
        _validatorMock.Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult(validationFailures));

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, default));
    }
}
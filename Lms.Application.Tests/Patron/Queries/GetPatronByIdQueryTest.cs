using Lms.Application.Patron.Queries;
using Lms.Domain.Interfaces.Repositories;
using Moq;
using Xunit;

namespace Lms.Application.Tests.Patron.Queries;

public class GetPatronByIdQueryTests
{
    private readonly Mock<IPatronRepository> _patronRepositoryMock;
    private readonly GetPatronByIdQueryHandler _handler;

    public GetPatronByIdQueryTests()
    {
        _patronRepositoryMock = new Mock<IPatronRepository>();
        _handler = new GetPatronByIdQueryHandler(_patronRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnPatron_WhenPatronExists()
    {
        // Arrange
        var patron = new Domain.Entities.Patron("John Doe");
        _patronRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(patron);
        var query = new GetPatronByIdQuery(1);

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(patron.Id, result.Id);
        Assert.Equal(patron.Name, result.Name);
    }

    [Fact]
    public async Task Handle_ShouldThrowKeyNotFoundException_WhenPatronDoesNotExist()
    {
        // Arrange
        _patronRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Domain.Entities.Patron)null);
        var query = new GetPatronByIdQuery(1);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(query, default));
    }
}
using Lms.Application.Patron.Queries;
using Lms.Domain.Interfaces.Repositories;
using Moq;
using Xunit;

namespace Lms.Application.Tests.Patron.Queries;

public class GetPatronsQueryTests
{
    private readonly Mock<IPatronRepository> _patronRepositoryMock;
    private readonly GetPatronsQueryHandler _handler;

    public GetPatronsQueryTests()
    {
        _patronRepositoryMock = new Mock<IPatronRepository>();
        _handler = new GetPatronsQueryHandler(_patronRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnListOfPatrons_WhenPatronsExist()
    {
        // Arrange
        var patrons = new List<Domain.Entities.Patron>
        {
            new Domain.Entities.Patron("John Doe"),
            new Domain.Entities.Patron("Jane Doe")
        };
        _patronRepositoryMock.Setup(r => r.GetPatronsAsync()).ReturnsAsync(patrons);
        var query = new GetPatronsQuery();

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(patrons[0].Id, result[0].Id);
        Assert.Equal(patrons[0].Name, result[0].Name);
        Assert.Equal(patrons[1].Id, result[1].Id);
        Assert.Equal(patrons[1].Name, result[1].Name);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoPatronsExist()
    {
        // Arrange
        _patronRepositoryMock.Setup(r => r.GetPatronsAsync()).ReturnsAsync(new List<Domain.Entities.Patron>());
        var query = new GetPatronsQuery();

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
using Lms.Application.Library.Queries;
using Lms.Domain.Interfaces;
using Lms.Domain.Interfaces.Repositories;
using Moq;
using Xunit;

namespace Lms.Application.Tests.Library.Queries;

public class GetLibrariesQueryTests
{
    private readonly Mock<ILibraryRepository> _libraryRepositoryMock;
    private readonly GetLibrariesQueryHandler _handler;

    public GetLibrariesQueryTests()
    {
        _libraryRepositoryMock = new Mock<ILibraryRepository>();
        _handler = new GetLibrariesQueryHandler(_libraryRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnListOfLibraries_WhenLibrariesExist()
    {
        // Arrange
        var libraries = new List<Domain.Aggregates.Library>
        {
            new Domain.Aggregates.Library("Library 1"),
            new Domain.Aggregates.Library("Library 2")
        };
        _libraryRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(libraries);
        var query = new GetLibrariesQuery();

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(libraries[0].Id, result[0].Id);
        Assert.Equal(libraries[0].Name, result[0].Name);
        Assert.Equal(libraries[1].Id, result[1].Id);
        Assert.Equal(libraries[1].Name, result[1].Name);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoLibrariesExist()
    {
        // Arrange
        _libraryRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Domain.Aggregates.Library>());
        var query = new GetLibrariesQuery();

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
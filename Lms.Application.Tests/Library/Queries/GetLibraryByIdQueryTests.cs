using Lms.Application.Library.Queries;
using Lms.Domain.Interfaces;
using Lms.Domain.Interfaces.Repositories;
using Moq;
using Xunit;

namespace Lms.Application.Tests.Library.Queries;

public class GetLibraryByIdQueryTests
{
    private readonly Mock<ILibraryRepository> _libraryRepositoryMock;
    private readonly GetLibraryByIdQueryHandler _handler;

    public GetLibraryByIdQueryTests()
    {
        _libraryRepositoryMock = new Mock<ILibraryRepository>();
        _handler = new GetLibraryByIdQueryHandler(_libraryRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnLibrary_WhenLibraryExists()
    {
        // Arrange
        var library = new Domain.Aggregates.Library("Existing Library");
        _libraryRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(library);
        var query = new GetLibraryByIdQuery(1);

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(library.Id, result.Id);
        Assert.Equal(library.Name, result.Name);
    }

    [Fact]
    public async Task Handle_ShouldThrowKeyNotFoundException_WhenLibraryDoesNotExist()
    {
        // Arrange
        _libraryRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Domain.Aggregates.Library)null);
        var query = new GetLibraryByIdQuery(1);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(query, default));
    }
}
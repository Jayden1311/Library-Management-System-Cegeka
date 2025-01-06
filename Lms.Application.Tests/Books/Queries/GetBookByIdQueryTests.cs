using Lms.Application.Books.Queries;
using Lms.Domain.Entities;
using Lms.Domain.Interfaces.Repositories;
using Moq;
using Xunit;

namespace Lms.Application.Tests.Books.Queries;

public class GetBookByIdQueryHandlerTests
{
    private readonly Mock<IBookRepository> _bookRepositoryMock;
    private readonly GetBookByIdQueryHandler _handler;

    public GetBookByIdQueryHandlerTests()
    {
        _bookRepositoryMock = new Mock<IBookRepository>();
        _handler = new GetBookByIdQueryHandler(_bookRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnBookDto_WhenBookExists()
    {
        // Arrange
        var query = new GetBookByIdQuery(1);
        var book = new Book("Title", "Author", "Genre", "1234567890123", new Domain.Aggregates.Library("Test Library"));
        _bookRepositoryMock.Setup(r => r.GetByIdAsync(query.BookId)).ReturnsAsync(book);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(book.Id, result.Id);
        Assert.Equal(book.Title, result.Title);
        Assert.Equal(book.Author, result.Author);
        Assert.Equal(book.Genre, result.Genre);
        Assert.Equal(book.ISBN, result.ISBN);
    }

    [Fact]
    public async Task Handle_ShouldThrowKeyNotFoundException_WhenBookDoesNotExist()
    {
        // Arrange
        var query = new GetBookByIdQuery(1);
        _bookRepositoryMock.Setup(r => r.GetByIdAsync(query.BookId)).ReturnsAsync((Book)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(query, CancellationToken.None));
    }
}
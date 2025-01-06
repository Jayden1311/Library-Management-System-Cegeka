using Lms.Application.Books.Queries;
using Lms.Domain.Entities;
using Lms.Domain.Interfaces.Repositories;
using Moq;
using Xunit;

namespace Lms.Application.Tests.Books.Queries;

public class GetBookByIsbnQueryTests
{
    private readonly Mock<IBookRepository> _bookRepositoryMock;
    private readonly GetBookByIsbnQueryHandler _handler;

    public GetBookByIsbnQueryTests()
    {
        _bookRepositoryMock = new Mock<IBookRepository>();
        _handler = new GetBookByIsbnQueryHandler(_bookRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnBookDto_WhenBookExists()
    {
        // Arrange
        var isbn = "1234567890123";
        var book = new Book("Title", "Author", "Genre", isbn, new Domain.Aggregates.Library("Test Library"));
        _bookRepositoryMock.Setup(r => r.GetByISBNAsync(isbn)).ReturnsAsync(book);

        var query = new GetBookByIsbnQuery(isbn);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(book.Id, result.Id);
        Assert.Equal(book.Title, result.Title);
        Assert.Equal(book.Author, result.Author);
        Assert.Equal(book.Genre, result.Genre);
        Assert.Equal(book.ISBN, result.ISBN);
        Assert.Equal(book.IsAvailable, result.IsAvailable);
        Assert.Equal(book.LibraryId, result.LibraryId);
    }

    [Fact]
    public async Task Handle_ShouldThrowKeyNotFoundException_WhenBookDoesNotExist()
    {
        // Arrange
        var isbn = "1234567890123";
        _bookRepositoryMock.Setup(r => r.GetByISBNAsync(isbn)).ReturnsAsync((Book)null);

        var query = new GetBookByIsbnQuery(isbn);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(query, CancellationToken.None));
    }
}
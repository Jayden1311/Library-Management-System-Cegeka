using Lms.Application.Books.Queries;
using Lms.Domain.Entities;
using Lms.Domain.Interfaces.Repositories;
using Moq;
using Xunit;

namespace Lms.Application.Tests.Books.Queries;

public class SearchBooksQueryHandlerTests
{
    private readonly Mock<IBookRepository> _bookRepositoryMock;
    private readonly SearchBooksQueryHandler _handler;

    public SearchBooksQueryHandlerTests()
    {
        _bookRepositoryMock = new Mock<IBookRepository>();
        _handler = new SearchBooksQueryHandler(_bookRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnListOfBookDtos_WhenBooksMatchKeyword()
    {
        // Arrange
        var query = new SearchBooksQuery("Title");
        var books = new List<Book>
        {
            new Book("Title1", "Author1", "Genre1", "1234567890123", new Domain.Aggregates.Library("Test Library")),
            new Book("Title2", "Author2", "Genre2", "1234567890124", new Domain.Aggregates.Library("Test Library"))
        };
        _bookRepositoryMock.Setup(r => r.SearchAsync(query.Keyword)).ReturnsAsync(books);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(books[0].Id, result[0].Id);
        Assert.Equal(books[1].Id, result[1].Id);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoBooksMatchKeyword()
    {
        // Arrange
        var query = new SearchBooksQuery("NonExistentTitle");
        _bookRepositoryMock.Setup(r => r.SearchAsync(query.Keyword)).ReturnsAsync(new List<Book>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
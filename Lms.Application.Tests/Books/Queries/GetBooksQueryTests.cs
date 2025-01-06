using Lms.Application.Books.Queries;
using Lms.Domain.Entities;
using Lms.Domain.Interfaces.Repositories;
using Moq;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lms.Application.Tests.Books.Queries;

public class GetBooksQueryTests
{
    [Fact]
    public async Task Handle_ReturnsAllBooks()
    {
        // Arrange
        var mockBookRepository = new Mock<IBookRepository>();
        mockBookRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(GetSampleBooks());

        var handler = new GetBooksQueryHandler(mockBookRepository.Object);

        // Act
        var result = await handler.Handle(new GetBooksQuery(), CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Book 1", result.First().Title);
    }

    private IEnumerable<Book> GetSampleBooks()
    {
        return new List<Book>
        {
            new Book("Book 1", "Author 1", "Genre 1", "1234567890", new Domain.Aggregates.Library("Library 1")),
            new Book("Book 2", "Author 2", "Genre 2", "0987654321", new Domain.Aggregates.Library("Library 2"))
        };
    }
}
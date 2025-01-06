using FluentAssertions;
using Lms.Domain.Aggregates;
using Lms.Domain.Entities;
using Lms.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Lms.Domain.IntegrationTests
{
    public class BookIntegrationTests
    {
        private readonly LmsDbContext _context;

        public BookIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<LmsDbContext>()
                .UseInMemoryDatabase(databaseName: "LibraryTestDb")
                .Options;
            _context = new LmsDbContext(options);
        }

        [Fact]
        public void AddBook_ShouldAddBookToDatabase()
        {
            // Arrange
            var library = new Library("Test Library");
            var book = new Book("Test Book", "Test Author", "Test Genre", "1234567890", library);

            _context.Libraries.Add(library);
            _context.Books.Add(book);
            _context.SaveChanges();

            // Act
            var retrievedBook = _context.Books.FirstOrDefault(b => b.ISBN == book.ISBN);

            // Assert
            retrievedBook.Should().NotBeNull();
            retrievedBook.Title.Should().Be("Test Book");
            retrievedBook.Author.Should().Be("Test Author");
            retrievedBook.Genre.Should().Be("Test Genre");
        }

        [Fact]
        public void UpdateBook_ShouldUpdateBookInDatabase()
        {
            // Arrange
            var library = new Library("Test Library");
            var book = new Book("Test Book", "Test Author", "Test Genre", "1234567891", library);

            _context.Libraries.Add(library);
            _context.Books.Add(book);
            _context.SaveChanges();

            // Act
            book.UpdateProperties("Updated Title", "Updated Author", "Updated Genre");
            _context.SaveChanges();

            // Assert
            var retrievedBook = _context.Books.FirstOrDefault(b => b.ISBN == book.ISBN);
            retrievedBook.Title.Should().Be("Updated Title");
            retrievedBook.Author.Should().Be("Updated Author");
            retrievedBook.Genre.Should().Be("Updated Genre");
        }

        [Fact]
        public void DeleteBook_ShouldRemoveBookFromDatabase()
        {
            // Arrange
            var library = new Library("Test Library");
            var book = new Book("Test Book", "Test Author", "Test Genre", "1234567892", library);

            _context.Libraries.Add(library);
            _context.Books.Add(book);
            _context.SaveChanges();

            // Act
            _context.Books.Remove(book);
            _context.SaveChanges();

            // Assert
            var retrievedBook = _context.Books.FirstOrDefault(b => b.ISBN == book.ISBN);
            retrievedBook.Should().BeNull();
        }
    }
}
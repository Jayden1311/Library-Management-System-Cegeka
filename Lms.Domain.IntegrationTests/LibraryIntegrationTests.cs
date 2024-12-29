using FluentAssertions;
using Lms.Domain.Aggregates;
using Lms.Domain.Entities;
using Lms.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Lms.Domain.IntegrationTests
{
    public class LibraryIntegrationTests
    {
        private readonly LibraryDbContext _context;

        public LibraryIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<LibraryDbContext>()
                .UseInMemoryDatabase(databaseName: "LibraryTestDb")
                .Options;
            _context = new LibraryDbContext(options);
        }

        [Fact]
        public void SearchBooks_ShouldReturnAvailableBooks()
        {
            // Arrange
            var library = new Library("Test Library");
            var availableBook = new Book("Available Book", "Author", "Genre", "1111111111", library);
            var checkedOutBook = new Book("Checked Out Book", "Author", "Genre", "2222222222", library);
            var patron = new Patron("Test Patron");

            _context.Libraries.Add(library);
            _context.Books.Add(availableBook);
            _context.Books.Add(checkedOutBook);
            _context.Patrons.Add(patron);
            _context.SaveChanges();

            
            library.CheckoutBook(checkedOutBook.ISBN, patron);
            _context.SaveChanges();

            // Act
            var searchResults = library.SearchBooks("Book");

            // Assert
            searchResults.Should().Contain(availableBook);
            searchResults.Should().Contain(checkedOutBook);
            availableBook.IsAvailable.Should().BeTrue();
            checkedOutBook.IsAvailable.Should().BeFalse();
        }

        [Fact]
        public void AddBook_ShouldAddBookToLibrary()
        {
            // Arrange
            var library = new Library("Test Library");
            var book = new Book("Test Book", "Test Author", "Test Genre", "1234567890", library);

            _context.Libraries.Add(library);
            _context.SaveChanges();

            // Act
            library.AddBook(book);
            _context.SaveChanges();

            // Assert
            var retrievedLibrary = _context.Libraries.Include(l => l.Books).FirstOrDefault(l => l.Id == library.Id);
            retrievedLibrary.Books.Should().Contain(book);
        }

        [Fact]
        public void RemoveBook_ShouldRemoveBookFromLibrary()
        {
            // Arrange
            var library = new Library("Test Library");
            var book = new Book("Test Book", "Test Author", "Test Genre", "1234567890", library);

            _context.Libraries.Add(library);
            _context.Books.Add(book);
            _context.SaveChanges();

            // Act
            library.RemoveBook(book.ISBN);
            _context.SaveChanges();

            // Assert
            var retrievedLibrary = _context.Libraries.Include(l => l.Books).FirstOrDefault(l => l.Id == library.Id);
            retrievedLibrary.Books.Should().NotContain(book);
        }
    }
}
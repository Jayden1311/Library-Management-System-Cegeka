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
        private readonly LmsDbContext _context;

        public LibraryIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<LmsDbContext>()
                .UseInMemoryDatabase(databaseName: "LibraryTestDb")
                .Options;
            _context = new LmsDbContext(options);
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

        [Fact]
        public void BookInOneLibrary_ShouldNotBeAvailableInAnotherLibrary()
        {
            // Arrange
            var library1 = new Library("Test Library 1");
            var library2 = new Library("Test Library 2");
            var book = new Book("Test Book", "Test Author", "Test Genre", "1234567890", library1);

            _context.Libraries.Add(library1);
            _context.Libraries.Add(library2);
            _context.Books.Add(book);
            _context.SaveChanges();

            // Act
            library1.RemoveBook(book.ISBN);
            _context.SaveChanges();

            // Assert
            var retrievedLibrary2 = _context.Libraries.Include(l => l.Books).FirstOrDefault(l => l.Id == library2.Id);
            retrievedLibrary2?.Books.Should().NotContain(book);
        }

        [Fact]
        public void BookInLibrary_ShouldBeAvailableInLibrary()
        {
            // Arrange
            var library1 = new Library("Test Library 1");
            var library2 = new Library("Test Library 2");
            var book = new Book("Test Book", "Test Author", "Test Genre", "1234567890", library1);

            _context.Libraries.Add(library1);
            _context.Libraries.Add(library2);
            _context.Books.Add(book);
            _context.SaveChanges();

            // Act
            library2.AddBook(book);
            _context.SaveChanges();

            // Assert
            var retrievedLibrary2 = _context.Libraries.Include(l => l.Books).FirstOrDefault(l => l.Id == library2.Id);
            retrievedLibrary2?.Books.Should().Contain(book);
        }
    }
}
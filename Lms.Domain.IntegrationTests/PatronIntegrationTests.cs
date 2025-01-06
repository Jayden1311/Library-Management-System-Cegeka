using FluentAssertions;
using Lms.Domain.Aggregates;
using Lms.Domain.Entities;
using Lms.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Lms.Domain.IntegrationTests
{
    public class PatronIntegrationTests
    {
        private readonly LmsDbContext _context;

        public PatronIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<LmsDbContext>()
                .UseInMemoryDatabase(databaseName: "LibraryTestDb")
                .Options;
            _context = new LmsDbContext(options);
        }

        [Fact]
        public void Patron_ShouldCheckoutAndReturnBook()
        {
            // Arrange
            var library = new Library("Test Library");
            var book = new Book("Test Book", "Test Author", "Test Genre", "1234567890", library);
            var patron = new Patron("Test Patron");

            _context.Libraries.Add(library);
            _context.Books.Add(book);
            _context.Patrons.Add(patron);
            _context.SaveChanges();

            // Act
            library.CheckoutBook(book.ISBN, patron);
            _context.SaveChanges();

            // Assert
            patron.CheckedOutBooks.Should().Contain(book);
            book.IsAvailable.Should().BeFalse();

            // Act
            library.ReturnBook(book.ISBN, patron);
            _context.SaveChanges();

            // Assert
            patron.CheckedOutBooks.Should().NotContain(book);
            book.IsAvailable.Should().BeTrue();
        }

        [Fact]
        public void Patron_ShouldNotCheckoutBook_WhenAlreadyCheckedOutByAnotherPatron()
        {
            // Arrange
            var library = new Library("Test Library");
            var book = new Book("Test Book", "Test Author", "Test Genre", "1234567890", library);
            var patron1 = new Patron("Test Patron 1");
            var patron2 = new Patron("Test Patron 2");

            _context.Libraries.Add(library);
            _context.Books.Add(book);
            _context.Patrons.Add(patron1);
            _context.Patrons.Add(patron2);
            _context.SaveChanges();

            // Act
            library.CheckoutBook(book.ISBN, patron1);
            _context.SaveChanges();

            // Assert
            patron1.CheckedOutBooks.Should().Contain(book);
            book.IsAvailable.Should().BeFalse();

            // Act & Assert
            Action act = () => library.CheckoutBook(book.ISBN, patron2);
            act.Should().Throw<InvalidOperationException>().WithMessage("Book is already checked out.");
        }

        [Fact]
        public void Patron_ShouldReturnBook_WhenCheckedOutByAnotherPatron()
        {
            // Arrange
            var library = new Library("Test Library");
            var book = new Book("Test Book", "Test Author", "Test Genre", "1234567890", library);
            var patron1 = new Patron("Test Patron 1");
            var patron2 = new Patron("Test Patron 2");

            _context.Libraries.Add(library);
            _context.Books.Add(book);
            _context.Patrons.Add(patron1);
            _context.Patrons.Add(patron2);
            _context.SaveChanges();

            // Act
            library.CheckoutBook(book.ISBN, patron1);
            _context.SaveChanges();

            // Assert
            patron1.CheckedOutBooks.Should().Contain(book);
            book.IsAvailable.Should().BeFalse();

            // Act
            library.ReturnBook(book.ISBN, patron1);
            _context.SaveChanges();

            // Assert
            patron1.CheckedOutBooks.Should().NotContain(book);
            book.IsAvailable.Should().BeTrue();

            // Act
            library.CheckoutBook(book.ISBN, patron2);
            _context.SaveChanges();

            // Assert
            patron2.CheckedOutBooks.Should().Contain(book);
            book.IsAvailable.Should().BeFalse();
        }

        [Fact]
        public void ReturnBook_ShouldThrowException_WhenBookNotCheckedOutByPatron()
        {
            // Arrange
            var library = new Library("Test Library");
            var book = new Book("Test Book", "Test Author", "Test Genre", "1234567890", library);
            var patron1 = new Patron("Test Patron 1");
            var patron2 = new Patron("Test Patron 2");

            _context.Libraries.Add(library);
            _context.Books.Add(book);
            _context.Patrons.Add(patron1);
            _context.Patrons.Add(patron2);
            _context.SaveChanges();

            library.CheckoutBook(book.ISBN, patron1);
            _context.SaveChanges();

            // Act
            Action act = () => library.ReturnBook(book.ISBN, patron2);

            // Assert
            act.Should().Throw<InvalidOperationException>().WithMessage("Book was not checked out by this patron.");
        }
    }
}
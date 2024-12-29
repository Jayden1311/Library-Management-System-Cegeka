using FluentAssertions;
using Lms.Domain.Aggregates;
using Lms.Domain.Entities;
using Moq;
using Xunit;

namespace Lms.Domain.Tests
{
    public class PatronTests
    {
        private readonly Mock<Library> _mockLibrary;
        private readonly Patron _testPatron;

        public PatronTests()
        {
            _mockLibrary = new Mock<Library>("Test Library");
            _testPatron = new Patron("Test2 Patron");
        }

        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var name = "Test Patron";

            // Act
            var patron = new Patron(name);

            // Assert
            patron.Name.Should().Be(name);
            patron.CheckedOutBooks.Should().BeEmpty();
        }

        [Fact]
        public void CheckoutBook_ShouldAddBookToCheckedOutBooks()
        {
            // Arrange
            var patron = new Patron("Test Patron");
            var mockBook = new Mock<Book>("Test Book", "Test Author", "Test Genre", "1234567890", _mockLibrary.Object);

            // Act
            patron.CheckoutBook(mockBook.Object);

            // Assert
            patron.CheckedOutBooks.Should().Contain(mockBook.Object);
        }

        [Fact]
        public void CheckoutBook_ShouldThrowException_WhenBookIsAlreadyCheckedOut()
        {
            // Arrange
            var patron = new Patron("Test Patron");
            var mockBook = new Mock<Book>("Test Book", "Test Author", "Test Genre", "1234567890", _mockLibrary.Object);
            _testPatron.CheckoutBook(mockBook.Object);

            // Act
            Action act = () => patron.CheckoutBook(mockBook.Object);

            // Assert
            act.Should().Throw<InvalidOperationException>().WithMessage("Book is already checked out.");
        }

        [Fact]
        public void ReturnBook_ShouldRemoveBookFromCheckedOutBooks()
        {
            // Arrange
            var patron = new Patron("Test Patron");
            var mockBook = new Mock<Book>("Test Book", "Test Author", "Test Genre", "1234567890", _mockLibrary.Object);
            patron.CheckoutBook(mockBook.Object);

            // Act
            patron.ReturnBook(mockBook.Object);

            // Assert
            patron.CheckedOutBooks.Should().NotContain(mockBook.Object);
        }

        [Fact]
        public void ReturnBook_ShouldThrowException_WhenBookIsNotCheckedOutByPatron()
        {
            // Arrange
            var patron = new Patron("Test Patron");
            var mockBook = new Mock<Book>("Test Book", "Test Author", "Test Genre", "1234567890", _mockLibrary.Object);

            // Act
            Action act = () => patron.ReturnBook(mockBook.Object);

            // Assert
            act.Should().Throw<InvalidOperationException>().WithMessage("This book was not checked out by this patron.");
        }
    }
}
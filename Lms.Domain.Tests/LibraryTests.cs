using FluentAssertions;
using Lms.Domain.Aggregates;
using Lms.Domain.Entities;
using Moq;
using Xunit;

namespace Lms.Domain.Tests
{
    public class LibraryTests
    {
        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var name = "Test Library";

            // Act
            var library = new Library(name);

            // Assert
            library.Name.Should().Be(name);
            library.Books.Should().BeEmpty();
        }

        [Fact]
        public void AddBook_ShouldAddBookToLibrary()
        {
            // Arrange
            var library = new Library("Test Library");
            var mockBook = new Mock<Book>("Test Book", "Test Author", "Test Genre", "1234567890", library);

            // Act
            library.AddBook(mockBook.Object);

            // Assert
            library.Books.Should().Contain(mockBook.Object);
        }

        [Fact]
        public void AddBook_ShouldThrowException_WhenBookWithSameISBNExists()
        {
            // Arrange
            var library = new Library("Test Library");
            var mockBook = new Mock<Book>("Test Book", "Test Author", "Test Genre", "1234567890", library);
            library.AddBook(mockBook.Object);

            // Act
            Action act = () => library.AddBook(mockBook.Object);

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("Book with the same ISBN already exists in the library.");
        }

        [Fact]
        public void RemoveBook_ShouldRemoveBookFromLibrary()
        {
            // Arrange
            var library = new Library("Test Library");
            var mockBook = new Mock<Book>("Test Book", "Test Author", "Test Genre", "1234567890", library);
            library.AddBook(mockBook.Object);

            // Act
            library.RemoveBook(mockBook.Object.ISBN);

            // Assert
            library.Books.Should().NotContain(mockBook.Object);
        }

        [Fact]
        public void RemoveBook_ShouldThrowException_WhenBookDoesNotExist()
        {
            // Arrange
            var library = new Library("Test Library");

            // Act
            Action act = () => library.RemoveBook("1234567890");

            // Assert
            act.Should().Throw<InvalidOperationException>().WithMessage("Book does not exist in the library.");
        }

        [Fact]
        public void CheckoutBook_ShouldSetBookAsCheckedOut()
        {
            // Arrange
            var library = new Library("Test Library");
            var mockBook = new Mock<Book>("Test Book", "Test Author", "Test Genre", "1234567890", library);
            var mockPatron = new Mock<Patron>("Test Patron");
            library.AddBook(mockBook.Object);

            // Act
            library.CheckoutBook(mockBook.Object.ISBN, mockPatron.Object);

            // Assert
            mockBook.Object.IsAvailable.Should().BeFalse();
            mockPatron.Object.CheckedOutBooks.Should().Contain(mockBook.Object);
        }

        [Fact]
        public void ReturnBook_ShouldSetBookAsAvailable()
        {
            // Arrange
            var library = new Library("Test Library");
            var mockBook = new Mock<Book>("Test Book", "Test Author", "Test Genre", "1234567890", library);
            var mockPatron = new Mock<Patron>("Test Patron");
            library.AddBook(mockBook.Object);
            library.CheckoutBook(mockBook.Object.ISBN, mockPatron.Object);

            // Act
            library.ReturnBook(mockBook.Object.ISBN, mockPatron.Object);

            // Assert
            mockBook.Object.IsAvailable.Should().BeTrue();
            mockPatron.Object.CheckedOutBooks.Should().NotContain(mockBook.Object);
        }
    }
}
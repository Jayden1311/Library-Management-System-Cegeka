using FluentAssertions;
using Lms.Domain.Aggregates;
using Lms.Domain.Entities;
using Moq;
using Xunit;

namespace Lms.Domain.Tests
{
    public class BookTests
    {
        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var title = "Test Book";
            var author = "Test Author";
            var genre = "Test Genre";
            var isbn = "1234567890";
            var mockLibrary = new Mock<Library>("Test Library");

            // Act
            var book = new Book(title, author, genre, isbn, mockLibrary.Object);

            // Assert
            book.Title.Should().Be(title);
            book.Author.Should().Be(author);
            book.Genre.Should().Be(genre);
            book.ISBN.Should().Be(isbn);
            book.Library.Should().Be(mockLibrary.Object);
        }

        [Fact]
        public void Constructor_ShouldSetIsAvailableToTrue()
        {
            // Arrange
            var mockLibrary = new Mock<Library>("Test Library");

            // Act
            var book = new Book("Test Book", "Test Author", "Test Genre", "1234567890", mockLibrary.Object);

            // Assert
            book.IsAvailable.Should().BeTrue();
        }

        [Fact]
        public void UpdateProperties_ShouldUpdateBookDetails()
        {
            // Arrange
            var mockLibrary = new Mock<Library>("Test Library");
            var book = new Book("Old Title", "Old Author", "Old Genre", "1234567890", mockLibrary.Object);

            // Act
            book.UpdateProperties("New Title", "New Author", "New Genre");

            // Assert
            book.Title.Should().Be("New Title");
            book.Author.Should().Be("New Author");
            book.Genre.Should().Be("New Genre");
        }

        [Fact]
        public void UpdateISBN_ShouldUpdateBookISBN()
        {
            // Arrange
            var mockLibrary = new Mock<Library>("Test Library");
            var book = new Book("Test Book", "Test Author", "Test Genre", "1234567890", mockLibrary.Object);

            // Act
            book.UpdateISBN("0987654321");

            // Assert
            book.ISBN.Should().Be("0987654321");
        }

        [Fact]
        public void MarkAsCheckedOut_ShouldSetIsAvailableToFalse()
        {
            // Arrange
            var mockLibrary = new Mock<Library>("Test Library");
            var book = new Book("Test Book", "Test Author", "Test Genre", "1234567890", mockLibrary.Object);

            // Act
            book.MarkAsCheckedOut();

            // Assert
            book.IsAvailable.Should().BeFalse();
        }

        [Fact]
        public void MarkAsReturned_ShouldSetIsAvailableToTrue()
        {
            // Arrange
            var mockLibrary = new Mock<Library>("Test Library");
            var book = new Book("Test Book", "Test Author", "Test Genre", "1234567890", mockLibrary.Object);
            book.MarkAsCheckedOut();

            // Act
            book.MarkAsReturned();

            // Assert
            book.IsAvailable.Should().BeTrue();
        }
    }
}
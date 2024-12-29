using FluentAssertions;
using Lms.Domain.Aggregates;
using Lms.Domain.Entities;
using Lms.Domain.Interfaces;
using Lms.Infrastructure.Persistence;
using Lms.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Lms.Infrastructure.Tests.Repositories
{
    public class BookRepositoryTests
    {
        private readonly LibraryDbContext _context;
        private readonly BookRepository _repository;

        public BookRepositoryTests()
        {
            // Set up an in-memory database
            var options = new DbContextOptionsBuilder<LibraryDbContext>()
                .UseInMemoryDatabase(databaseName: "LibraryTestDb")
                .Options;

            _context = new LibraryDbContext(options);
            Mock<ILogger<BookRepository>> mockLogger = new();
            _repository = new BookRepository(_context, mockLogger.Object);
        }

        [Fact]
        public async Task AddAsync_ShouldAddBook()
        {
            // Arrange
            var library = new Library("Test Library");
            _context.Libraries.Add(library);
            await _context.SaveChangesAsync();

            var book = new Book("Test Book", "Author", "Fiction", "1234567899", library);

            // Act
            await _repository.AddAsync(book);
            await _repository.SaveChangesAsync();

            // Assert
            var addedBook = await _context.Books.Include(b => b.Library)
                .FirstOrDefaultAsync(b => b.ISBN == "1234567899");
            Assert.NotNull(addedBook);
            Assert.Equal("Test Book", addedBook.Title);
            Assert.Equal(library.Id, addedBook.Library.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnBook_WhenBookExists()
        {
            // Arrange
            var library = new Library("Test Library");
            _context.Libraries.Add(library);
            await _context.SaveChangesAsync();

            var book = new Book("Existing Book", "Author", "Non-Fiction", "0987654321", library);

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(book.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Existing Book", result.Title);
            Assert.Equal(library.Id, result.Library.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowException_WhenBookDoesNotExist()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _repository.GetByIdAsync(99));
            Assert.Equal("Book with ID 99 not found.", exception.Message);
        }

        [Fact]
        public async Task GetByISBNAsync_ShouldReturnBook_WhenBookExists()
        {
            // Arrange
            var library = new Library("Test Library");
            _context.Libraries.Add(library);
            await _context.SaveChangesAsync();

            var book = new Book("ISBN Book", "Author", "Fiction", "1122334455", library);

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByISBNAsync("1122334455");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("ISBN Book", result.Title);
            Assert.Equal(library.Id, result.Library.Id);
        }

        [Fact]
        public async Task GetByISBNAsync_ShouldThrowException_WhenBookDoesNotExist()
        {
            // Act & Assert
            var exception =
                await Assert.ThrowsAsync<KeyNotFoundException>(() => _repository.GetByISBNAsync("0000000000"));
            Assert.Equal("Book with ISBN 0000000000 not found.", exception.Message);
        }

        [Fact]
        public async Task SearchAsync_ShouldReturnBooks_MatchingKeyword()
        {
            // Arrange
            var library = new Library("Test Library");
            _context.Libraries.Add(library);
            await _context.SaveChangesAsync();

            var books = new[]
            {
                new Book("Searchable Book", "Author", "Fiction", "5566778899", library),
                new Book("Another Book", "Search Author", "Non-Fiction", "6677889900", library)
            };

            _context.Books.AddRange(books);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.SearchAsync("Search");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task SaveChangesAsync_ShouldSaveChangesToDatabase()
        {
            // Arrange
            var library = new Library("Test Library");
            _context.Libraries.Add(library);
            await _context.SaveChangesAsync();

            var book = new Book("Save Book", "Author", "Fiction", "1234567890", library);

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            // Act
            book.UpdateProperties("Updated Book", "Author", "Fiction");
            await _repository.SaveChangesAsync();

            // Assert
            var updatedBook = await _context.Books.Include(b => b.Library)
                .FirstOrDefaultAsync(b => b.ISBN == "1234567890");
            Assert.NotNull(updatedBook);
            Assert.Equal("Updated Book", updatedBook.Title);
        }
    }
}
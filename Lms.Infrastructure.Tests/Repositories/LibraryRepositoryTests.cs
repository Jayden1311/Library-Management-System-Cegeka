using Lms.Domain.Aggregates;
using Lms.Infrastructure.Persistence;
using Lms.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace Lms.Infrastructure.Tests.Repositories
{
    public class LibraryRepositoryTests
    {
        private readonly LibraryRepository _repository;
        private readonly LmsDbContext _context;
        private readonly ILogger<LibraryRepository> _logger;

        public LibraryRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<LmsDbContext>()
                .UseInMemoryDatabase(databaseName: "LibraryTestDb")
                .Options;

            _context = new LmsDbContext(options);
            _logger = new LoggerFactory().CreateLogger<LibraryRepository>();
            _repository = new LibraryRepository(_context, _logger);
        }

        [Fact]
        public async Task AddAsync_ShouldAddLibrary()
        {
            // Arrange
            var library = new Library("Test Library");

            // Act
            await _repository.AddAsync(library);
            await _repository.SaveChangesAsync();

            // Assert
            var addedLibrary = await _context.Libraries.FindAsync(library.Id);
            Assert.NotNull(addedLibrary);
            Assert.Equal(library.Name, addedLibrary.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnLibrary_WhenLibraryExists()
        {
            // Arrange
            var library = new Library("Test Library");
            await _context.Libraries.AddAsync(library);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(library.Id);

            // Assert
            Assert.Equal(library, result);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowKeyNotFoundException_WhenLibraryDoesNotExist()
        {
            // Arrange
            var library = new Library("Test Library");
            await _context.Libraries.AddAsync(library);
            await _context.SaveChangesAsync();

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _repository.GetByIdAsync(0));
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllLibraries()
        {
            // Arrange
            var library1 = new Library("Test Library 1");
            var library2 = new Library("Test Library 2");
            await _context.Libraries.AddRangeAsync(library1, library2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.Contains(library1, result);
            Assert.Contains(library2, result);
        }

        [Fact]
        public async Task SaveChangesAsync_ShouldSaveChanges()
        {
            // Arrange
            var library = new Library("Test Library");
            await _repository.AddAsync(library);

            // Act
            await _repository.SaveChangesAsync();

            // Assert
            var addedLibrary = await _context.Libraries.FindAsync(library.Id);
            Assert.NotNull(addedLibrary);
        }
    }
}
using Lms.Domain.Entities;
using Lms.Infrastructure.Persistence;
using Lms.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Lms.Infrastructure.Tests.Repositories;

public class PatronRepositoryTests
{
    private readonly LibraryDbContext _context;
    private readonly PatronRepository _repository;

    public PatronRepositoryTests()
    {
        // Set up an in-memory database
        var options = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseInMemoryDatabase(databaseName: "LibraryTestDb")
            .Options;

        _context = new LibraryDbContext(options);
        Mock<ILogger<PatronRepository>> mockLogger = new();
        _repository = new PatronRepository(_context, mockLogger.Object);
    }

    [Fact]
    public async Task AddAsync_ShouldAddPatron()
    {
        // Arrange
        var patron = new Patron("Test Patron");

        // Act
        await _repository.AddAsync(patron);
        await _repository.SaveChangesAsync();

        // Assert
        var addedPatron = await _context.Patrons.FirstOrDefaultAsync(p => p.Name == "Test Patron");
        Assert.NotNull(addedPatron);
        Assert.Equal("Test Patron", addedPatron.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnPatron_WhenPatronExists()
    {
        // Arrange
        var patron = new Patron("Existing Patron");

        _context.Patrons.Add(patron);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(patron.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Existing Patron", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowException_WhenPatronDoesNotExist()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _repository.GetByIdAsync(99));
        Assert.Equal("Patron with ID 99 not found.", exception.Message);
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldSaveChangesToDatabase()
    {
        // Arrange
        var patron = new Patron("Save Patron");

        _context.Patrons.Add(patron);
        await _context.SaveChangesAsync();

        // Act
        patron.UpdateName("Updated Patron");
        await _repository.SaveChangesAsync();

        // Assert
        var updatedPatron = await _context.Patrons.FirstOrDefaultAsync(p => p.Name == "Updated Patron");
        Assert.NotNull(updatedPatron);
        Assert.Equal("Updated Patron", updatedPatron.Name);
    }
}
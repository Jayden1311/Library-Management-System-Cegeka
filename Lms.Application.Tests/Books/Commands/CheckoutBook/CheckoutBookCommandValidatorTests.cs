using FluentValidation.TestHelper;
using Lms.Application.Books.Commands.CheckoutBook;
using Lms.Domain.Entities;
using Lms.Domain.Interfaces.Repositories;
using Moq;
using Xunit;

namespace Lms.Application.Tests.Books.Commands.CheckoutBook;

public class CheckoutBookCommandValidatorTests
{
    private readonly Mock<IPatronRepository> _patronRepositoryMock;
    private readonly Mock<IBookRepository> _bookRepositoryMock;
    private readonly Mock<ILibraryRepository> _libraryRepositoryMock;
    private readonly CheckoutBookCommandValidator _validator;

    public CheckoutBookCommandValidatorTests()
    {
        _patronRepositoryMock = new Mock<IPatronRepository>();
        _bookRepositoryMock = new Mock<IBookRepository>();
        _libraryRepositoryMock = new Mock<ILibraryRepository>();
        _validator = new CheckoutBookCommandValidator(_patronRepositoryMock.Object, _bookRepositoryMock.Object,
            _libraryRepositoryMock.Object);
    }

    [Fact]
    public async Task Should_Have_Error_When_PatronId_Is_Zero()
    {
        var command = new CheckoutBookCommand(0, 1, "1234567890");
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.PatronId);
    }

    [Fact]
    public async Task Should_Have_Error_When_ISBN_Is_Empty()
    {
        var command = new CheckoutBookCommand(1, 1, string.Empty);
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.ISBN);
    }

    [Fact]
    public async Task Should_Have_Error_When_Patron_Does_Not_Exist()
    {
        _patronRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Domain.Entities.Patron?)null);

        var command = new CheckoutBookCommand(1, 1, "1234567890");
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.PatronId).WithErrorMessage("Patron does not exist.");
    }

    [Fact]
    public async Task Should_Have_Error_When_Book_Does_Not_Exist()
    {
        _bookRepositoryMock.Setup(repo => repo.GetByISBNAsync(It.IsAny<string>())).ReturnsAsync((Book?)null);

        var command = new CheckoutBookCommand(1, 1, "1234567890");
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.ISBN).WithErrorMessage("Book does not exist.");
    }

    [Fact]
    public async Task Should_Have_Error_When_Book_Is_Not_Available_At_Specified_Library()
    {
        var library = new Domain.Aggregates.Library("Test Library");
        _libraryRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(library);
        _bookRepositoryMock.Setup(repo => repo.GetByISBNAsync(It.IsAny<string>()))
            .ReturnsAsync(new Book("Test Title", "Test Author", "Test Genre", "1234567890", library));

        var command = new CheckoutBookCommand(1, 1, "1234567890");
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.ISBN)
            .WithErrorMessage("Book is not available at the specified library.");
    }
}
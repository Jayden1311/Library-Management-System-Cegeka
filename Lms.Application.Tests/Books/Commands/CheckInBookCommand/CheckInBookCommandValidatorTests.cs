using FluentValidation;
using FluentValidation.TestHelper;
using Lms.Application.Books.Commands.CheckinBook;
using Lms.Domain.Interfaces.Repositories;
using Moq;
using Xunit;

namespace Lms.Application.Tests.Books.Commands.CheckinBook
{
    public class CheckinBookCommandValidatorTests
    {
        private readonly CheckinBookCommandValidator _validator;
        private readonly Mock<IPatronRepository> _patronRepositoryMock;
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        private readonly Mock<ILibraryRepository> _libraryRepositoryMock;

        public CheckinBookCommandValidatorTests()
        {
            _patronRepositoryMock = new Mock<IPatronRepository>();
            _bookRepositoryMock = new Mock<IBookRepository>();
            _libraryRepositoryMock = new Mock<ILibraryRepository>();

            _validator = new CheckinBookCommandValidator(_patronRepositoryMock.Object, _bookRepositoryMock.Object,
                _libraryRepositoryMock.Object);
        }

        [Fact]
        public async Task Should_Have_Error_When_PatronId_Is_Invalid()
        {
            var command = new CheckinBookCommand(0, 1, "1234567890");
            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.PatronId);
        }

        [Fact]
        public async Task Should_Have_Error_When_ISBN_Is_Empty()
        {
            var command = new CheckinBookCommand(1, 1, string.Empty);
            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.ISBN);
        }

        [Fact]
        public async Task Should_Have_Error_When_Patron_Does_Not_Exist()
        {
            _patronRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Lms.Domain.Entities.Patron?)null);
            var command = new CheckinBookCommand(1, 1, "1234567890");
            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.PatronId).WithErrorMessage("Patron does not exist.");
        }

        [Fact]
        public async Task Should_Have_Error_When_Book_Does_Not_Exist()
        {
            _bookRepositoryMock.Setup(repo => repo.GetByISBNAsync(It.IsAny<string>()))
                .ReturnsAsync((Lms.Domain.Entities.Book?)null);
            var command = new CheckinBookCommand(1, 1, "1234567890");
            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.ISBN).WithErrorMessage("Book does not exist.");
        }

        [Fact]
        public async Task Should_Have_Error_When_Book_Is_Not_Checked_Out()
        {
            var library = new Lms.Domain.Aggregates.Library("Test Library");
            var book = new Lms.Domain.Entities.Book("Test Title", "Test Author", "Test Genre", "1234567890", library);
            _libraryRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(library);
            _bookRepositoryMock.Setup(repo => repo.GetByISBNAsync(It.IsAny<string>())).ReturnsAsync(book);

            var command = new CheckinBookCommand(1, 1, "1234567890");
            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.ISBN)
                .WithErrorMessage("Book is not checked out from the specified library.");
        }
    }
}
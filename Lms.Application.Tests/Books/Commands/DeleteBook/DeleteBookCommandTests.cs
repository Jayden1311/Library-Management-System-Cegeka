using FluentValidation;
using FluentValidation.Results;
using Lms.Application.Books.Commands.DeleteBook;
using Lms.Domain.Entities;
using Lms.Domain.Interfaces.Repositories;
using Moq;
using Xunit;

namespace Lms.Application.Tests.Books.Commands.DeleteBook
{
    public class DeleteBookCommandHandlerTests
    {
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        private readonly Mock<IValidator<DeleteBookCommand>> _validatorMock;
        private readonly DeleteBookCommandHandler _handler;

        public DeleteBookCommandHandlerTests()
        {
            _bookRepositoryMock = new Mock<IBookRepository>();
            _validatorMock = new Mock<IValidator<DeleteBookCommand>>();
            _handler = new DeleteBookCommandHandler(_bookRepositoryMock.Object, _validatorMock.Object);
        }

        [Fact]
        public async Task Handle_Should_Throw_Exception_When_Validation_Fails()
        {
            var command = new DeleteBookCommand(1);
            _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(
                new ValidationResult(new List<ValidationFailure>
                    { new ValidationFailure("Id", "Invalid book ID.") }));

            await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_Should_Throw_Exception_When_Book_Not_Found()
        {
            var command = new DeleteBookCommand(1);
            _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
            _bookRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Book?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_Should_Delete_Book_When_Valid()
        {
            var command = new DeleteBookCommand(1);
            var book = new Book("Test Title", "Test Author", "Test Genre", "1234567890", new Domain.Aggregates.Library("Test Library"));

            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<DeleteBookCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
            _bookRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(book);
            _bookRepositoryMock.Setup(repo => repo.DeleteAsync(It.IsAny<Book>())).Returns(Task.CompletedTask);

            await _handler.Handle(command, CancellationToken.None);

            _bookRepositoryMock.Verify(repo => repo.DeleteAsync(book), Times.Once);
            _bookRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }
    }
}
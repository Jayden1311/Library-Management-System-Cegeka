using FluentValidation.TestHelper;
using Lms.Application.Books.Commands.DeleteBook;
using Xunit;

namespace Lms.Application.Tests.Books.Commands.DeleteBook
{
    public class DeleteBookCommandValidatorTests
    {
        private readonly DeleteBookCommandValidator _validator;

        public DeleteBookCommandValidatorTests()
        {
            _validator = new DeleteBookCommandValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Id_Is_Less_Than_Or_Equal_To_Zero()
        {
            var command = new DeleteBookCommand(0);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Command_Is_Valid()
        {
            var command = new DeleteBookCommand(1);
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveValidationErrorFor(x => x.Id);
        }
    }
}
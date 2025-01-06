using FluentValidation.TestHelper;
using Lms.Application.Patron.Commands.DeletePatron;
using Xunit;

namespace Lms.Application.Tests.Patron.Commands.DeletePatron;

public class DeletePatronCommandValidatorTests
{
    private readonly DeletePatronCommandValidator _validator;

    public DeletePatronCommandValidatorTests()
    {
        _validator = new DeletePatronCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Id_Is_Less_Than_Or_Equal_To_Zero()
    {
        var command = new DeletePatronCommand(0);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.Id);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Id_Is_Greater_Than_Zero()
    {
        var command = new DeletePatronCommand(1);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(c => c.Id);
    }
}
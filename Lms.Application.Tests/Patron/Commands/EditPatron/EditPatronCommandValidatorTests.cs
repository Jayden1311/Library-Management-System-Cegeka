using FluentValidation.TestHelper;
using Lms.Application.Patron.Commands;
using Xunit;

namespace Lms.Application.Tests.Patron.Commands.EditPatron;

public class EditPatronCommandValidatorTests
{
    private readonly EditPatronCommandValidator _validator;

    public EditPatronCommandValidatorTests()
    {
        _validator = new EditPatronCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Id_Is_Less_Than_Or_Equal_To_Zero()
    {
        var command = new EditPatronCommand(0, "John Doe");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.Id);
    }

    [Fact]
    public void Should_Have_Error_When_Name_Is_Empty()
    {
        var command = new EditPatronCommand(1, string.Empty);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.Name);
    }

    [Fact]
    public void Should_Have_Error_When_Name_Exceeds_Max_Length()
    {
        var command = new EditPatronCommand(1, new string('a', 101));
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.Name);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Command_Is_Valid()
    {
        var command = new EditPatronCommand(1, "John Doe");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(c => c.Id);
        result.ShouldNotHaveValidationErrorFor(c => c.Name);
    }
}
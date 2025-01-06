using FluentValidation.TestHelper;
using Lms.Application.Library.Commands.EditLibrary;
using Xunit;

namespace Lms.Application.Tests.Library.Commands.EditLibrary;

public class EditLibraryCommandValidatorTests
{
    private readonly EditLibraryCommandValidator _validator;

    public EditLibraryCommandValidatorTests()
    {
        _validator = new EditLibraryCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Id_Is_Less_Than_Or_Equal_To_Zero()
    {
        var command = new EditLibraryCommand(0, "Valid Name");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Should_Have_Error_When_Name_Is_Empty()
    {
        var command = new EditLibraryCommand(1, string.Empty);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Have_Error_When_Name_Exceeds_Max_Length()
    {
        var command = new EditLibraryCommand(1, new string('a', 101));
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Command_Is_Valid()
    {
        var command = new EditLibraryCommand(1, "Valid Name");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }
}
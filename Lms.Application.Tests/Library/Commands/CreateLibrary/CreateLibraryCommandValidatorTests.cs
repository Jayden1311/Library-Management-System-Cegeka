using FluentValidation.TestHelper;
using Lms.Application.Library.Commands.CreateLibrary;
using Xunit;

namespace Lms.Application.Tests.Library.Commands.CreateLibrary;

public class CreateLibraryCommandValidatorTests
{
    private readonly CreateLibraryCommandValidator _validator;

    public CreateLibraryCommandValidatorTests()
    {
        _validator = new CreateLibraryCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Name_Is_Empty()
    {
        var command = new CreateLibraryCommand(string.Empty);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Have_Error_When_Name_Exceeds_Max_Length()
    {
        var command = new CreateLibraryCommand(new string('a', 101));
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Name_Is_Valid()
    {
        var command = new CreateLibraryCommand("Valid Name");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }
}
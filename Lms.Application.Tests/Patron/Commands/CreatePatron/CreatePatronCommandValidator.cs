using FluentValidation.TestHelper;
using Lms.Application.Patron.Commands.CreatePatronCommand;
using Xunit;

namespace Lms.Application.Tests.Patron.Commands.CreatePatron;

public class CreatePatronCommandValidatorTests
{
    private readonly CreatePatronCommandValidator _validator;

    public CreatePatronCommandValidatorTests()
    {
        _validator = new CreatePatronCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Name_Is_Empty()
    {
        var command = new CreatePatronCommand(string.Empty);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.Name);
    }

    [Fact]
    public void Should_Have_Error_When_Name_Exceeds_Max_Length()
    {
        var command = new CreatePatronCommand(new string('a', 101));
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.Name);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Name_Is_Valid()
    {
        var command = new CreatePatronCommand("John Doe");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(c => c.Name);
    }
}
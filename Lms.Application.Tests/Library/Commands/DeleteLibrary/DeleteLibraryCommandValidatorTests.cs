using FluentValidation.TestHelper;
using Lms.Application.Library.Commands;
using Lms.Application.Library.Commands.DeleteLibrary;
using Xunit;

namespace Lms.Application.Tests.Library.Commands.DeleteLibrary;

public class DeleteLibraryCommandValidatorTests
{
    private readonly DeleteLibraryCommandValidator _validator;

    public DeleteLibraryCommandValidatorTests()
    {
        _validator = new DeleteLibraryCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Id_Is_Less_Than_Or_Equal_To_Zero()
    {
        var command = new DeleteLibraryCommand(0);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Id_Is_Valid()
    {
        var command = new DeleteLibraryCommand(1);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }
}
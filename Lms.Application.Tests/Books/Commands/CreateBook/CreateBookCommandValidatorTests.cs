using FluentValidation.TestHelper;
using Lms.Application.Books.Commands.CreateBook;
using Xunit;

namespace Lms.Application.Tests.Books.Commands.CreateBook;

public class CreateBookCommandValidatorTests
{
    private readonly CreateBookCommandValidator _validator;

    public CreateBookCommandValidatorTests()
    {
        _validator = new CreateBookCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_LibraryId_Is_Less_Than_Or_Equal_To_Zero()
    {
        var command = new CreateBookCommand(0, "Title", "Author", "Genre", "1234567890123");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.LibraryId);
    }

    [Fact]
    public void Should_Have_Error_When_Title_Is_Empty()
    {
        var command = new CreateBookCommand(1, "", "Author", "Genre", "1234567890123");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Should_Have_Error_When_Title_Exceeds_Max_Length()
    {
        var command = new CreateBookCommand(1, new string('a', 101), "Author", "Genre", "1234567890123");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Should_Have_Error_When_Author_Is_Empty()
    {
        var command = new CreateBookCommand(1, "Title", "", "Genre", "1234567890123");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Author);
    }

    [Fact]
    public void Should_Have_Error_When_Author_Exceeds_Max_Length()
    {
        var command = new CreateBookCommand(1, "Title", new string('a', 101), "Genre", "1234567890123");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Author);
    }

    [Fact]
    public void Should_Have_Error_When_Genre_Is_Empty()
    {
        var command = new CreateBookCommand(1, "Title", "Author", "", "1234567890123");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Genre);
    }

    [Fact]
    public void Should_Have_Error_When_Genre_Exceeds_Max_Length()
    {
        var command = new CreateBookCommand(1, "Title", "Author", new string('a', 51), "1234567890123");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Genre);
    }

    [Fact]
    public void Should_Have_Error_When_ISBN_Is_Empty()
    {
        var command = new CreateBookCommand(1, "Title", "Author", "Genre", "");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ISBN);
    }

    [Fact]
    public void Should_Have_Error_When_ISBN_Exceeds_Max_Length()
    {
        var command = new CreateBookCommand(1, "Title", "Author", "Genre", new string('a', 14));
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ISBN);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Command_Is_Valid()
    {
        var command = new CreateBookCommand(1, "Title", "Author", "Genre", "1234567890123");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.LibraryId);
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
        result.ShouldNotHaveValidationErrorFor(x => x.Author);
        result.ShouldNotHaveValidationErrorFor(x => x.Genre);
        result.ShouldNotHaveValidationErrorFor(x => x.ISBN);
    }
}
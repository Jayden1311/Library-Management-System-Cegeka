using FluentValidation.TestHelper;
using Lms.Application.Books.Commands.EditBook;
using Xunit;

namespace Lms.Application.Tests.Books.Commands.EditBook;

public class EditBookCommandValidatorTests
{
    private readonly EditBookCommandValidator _validator;

    public EditBookCommandValidatorTests()
    {
        _validator = new EditBookCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_LibraryId_Is_Less_Than_Or_Equal_To_Zero()
    {
        var command = new EditBookCommand(0, 1, "Title", "Author", "Genre", "1234567890123");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.LibraryId);
    }

    [Fact]
    public void Should_Have_Error_When_BookId_Is_Less_Than_Or_Equal_To_Zero()
    {
        var command = new EditBookCommand(1, 0, "Title", "Author", "Genre", "1234567890123");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.BookId);
    }

    [Fact]
    public void Should_Have_Error_When_Title_Is_Empty()
    {
        var command = new EditBookCommand(1, 1, "", "Author", "Genre", "1234567890123");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Should_Have_Error_When_Title_Exceeds_Max_Length()
    {
        var command = new EditBookCommand(1, 1, new string('a', 101), "Author", "Genre", "1234567890123");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Should_Have_Error_When_Author_Is_Empty()
    {
        var command = new EditBookCommand(1, 1, "Title", "", "Genre", "1234567890123");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Author);
    }

    [Fact]
    public void Should_Have_Error_When_Author_Exceeds_Max_Length()
    {
        var command = new EditBookCommand(1, 1, "Title", new string('a', 101), "Genre", "1234567890123");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Author);
    }

    [Fact]
    public void Should_Have_Error_When_Genre_Is_Empty()
    {
        var command = new EditBookCommand(1, 1, "Title", "Author", "", "1234567890123");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Genre);
    }

    [Fact]
    public void Should_Have_Error_When_Genre_Exceeds_Max_Length()
    {
        var command = new EditBookCommand(1, 1, "Title", "Author", new string('a', 51), "1234567890123");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Genre);
    }

    [Fact]
    public void Should_Have_Error_When_ISBN_Is_Empty()
    {
        var command = new EditBookCommand(1, 1, "Title", "Author", "Genre", "");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ISBN);
    }

    [Fact]
    public void Should_Have_Error_When_ISBN_Exceeds_Max_Length()
    {
        var command = new EditBookCommand(1, 1, "Title", "Author", "Genre", new string('a', 14));
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ISBN);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Command_Is_Valid()
    {
        var command = new EditBookCommand(1, 1, "Title", "Author", "Genre", "1234567890123");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.LibraryId);
        result.ShouldNotHaveValidationErrorFor(x => x.BookId);
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
        result.ShouldNotHaveValidationErrorFor(x => x.Author);
        result.ShouldNotHaveValidationErrorFor(x => x.Genre);
        result.ShouldNotHaveValidationErrorFor(x => x.ISBN);
    }
}
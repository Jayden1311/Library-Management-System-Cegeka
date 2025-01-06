using System.ComponentModel;
using FluentValidation;
using Lms.Application.Books.Commands.CheckinBook;
using Lms.Application.Books.Commands.CheckoutBook;
using Lms.Application.Books.Commands.CreateBook;
using Lms.Application.Books.Commands.DeleteBook;
using Lms.Application.Books.Commands.EditBook;
using Lms.Application.Books.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lms.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookController : ControllerBase
{
    private readonly ISender _mediator;

    public BookController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    [Description("Gets a book by id")]
    [ProducesResponseType(typeof(BookDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookDto>> GetBookById([FromRoute] int id)
    {
        try
        {
            var query = new GetBookByIdQuery(id);
            var book = await _mediator.Send(query);
            return Ok(book);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("isbn/{isbn}")]
    [Description("Gets a book by ISBN")]
    [ProducesResponseType(typeof(BookDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookDto>> GetBookByIsbn([FromRoute] string isbn)
    {
        try
        {
            var query = new GetBookByIsbnQuery(isbn);
            var book = await _mediator.Send(query);
            return Ok(book);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("search")]
    [Description("Searches books by keyword")]
    [ProducesResponseType(typeof(List<BookDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<BookDto>>> SearchBooks([FromQuery] string keyword)
    {
        var query = new SearchBooksQuery(keyword);
        var books = await _mediator.Send(query);
        return Ok(books);
    }

    [HttpGet]
    [Description("Gets all books")]
    [ProducesResponseType(typeof(List<BookDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<BookDto>>> GetBooks()
    {
        var query = new GetBooksQuery();
        var books = await _mediator.Send(query);
        return Ok(books);
    }

    [HttpPost]
    [Description("Creates a new book")]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<int>> CreateBook([FromBody] CreateBookCommand command)
    {
        try
        {
            var bookId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetBookById), new { id = bookId }, bookId);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    [Description("Edits an existing book")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EditBook([FromRoute] int id, [FromBody] EditBookCommand command)
    {
        if (id != command.BookId)
        {
            return BadRequest("ID in route does not match ID in command");
        }

        try
        {
            await _mediator.Send(command);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [Description("Deletes a book")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteBook([FromRoute] int id)
    {
        try
        {
            var command = new DeleteBookCommand(id);
            await _mediator.Send(command);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost("checkout")]
    [Description("Checks out a book")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CheckoutBook([FromBody] CheckoutBookCommand command)
    {
        try
        {
            await _mediator.Send(command);
            return NoContent();
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("checkin")]
    [Description("Checks in a book")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CheckinBook([FromBody] CheckinBookCommand command)
    {
        try
        {
            await _mediator.Send(command);
            return NoContent();
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
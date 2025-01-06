using System.ComponentModel;
using Lms.Application.Library.Commands;
using Lms.Application.Library.Commands.CreateLibrary;
using Lms.Application.Library.Commands.EditLibrary;
using Lms.Application.Library.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lms.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LibraryController : ControllerBase
{
    private readonly ISender _mediator;

    public LibraryController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Description("Gets all libraries")]
    [ProducesResponseType(typeof(List<LibraryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<LibraryDto>>> GetLibraries()
    {
        var query = new GetLibrariesQuery();
        var libraries = await _mediator.Send(query);
        return Ok(libraries);
    }

    [HttpGet("{id}")]
    [Description("Gets a library by id")]
    [ProducesResponseType(typeof(LibraryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LibraryDto>> GetLibraryById([FromRoute] int id)
    {
        try
        {
            var query = new GetLibraryByIdQuery(id);
            var library = await _mediator.Send(query);
            return Ok(library);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost]
    [Description("Creates a new library")]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    public async Task<ActionResult<int>> CreateLibrary([FromBody] CreateLibraryCommand command)
    {
        var libraryId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetLibraryById), new { id = libraryId }, libraryId);
    }

    [HttpPut("{id}")]
    [Description("Edits an existing library")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EditLibrary([FromRoute] int id, [FromBody] EditLibraryCommand command)
    {
        if (id != command.Id)
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
    [Description("Deletes a library")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteLibrary([FromRoute] int id)
    {
        try
        {
            var command = new DeleteLibraryCommand(id);
            await _mediator.Send(command);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
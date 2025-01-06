using System.ComponentModel;
using Lms.Application.Patron.Commands;
using Lms.Application.Patron.Commands.CreatePatronCommand;
using Lms.Application.Patron.Commands.DeletePatron;
using Lms.Application.Patron.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lms.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatronController : ControllerBase
{
    private readonly ISender _mediator;

    public PatronController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Description("Gets all patrons")]
    [ProducesResponseType(typeof(List<PatronDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PatronDto>>> GetPatrons()
    {
        var query = new GetPatronsQuery();
        var patrons = await _mediator.Send(query);
        return Ok(patrons);
    }

    [HttpGet("{id}")]
    [Description("Gets a patron by id")]
    [ProducesResponseType(typeof(PatronDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PatronDto>> GetPatronById([FromRoute] int id)
    {
        try
        {
            var query = new GetPatronByIdQuery(id);
            var patron = await _mediator.Send(query);
            return Ok(patron);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost]
    [Description("Creates a new patron")]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    public async Task<ActionResult<int>> CreatePatron([FromBody] CreatePatronCommand command)
    {
        var patronId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetPatronById), new { id = patronId }, patronId);
    }

    [HttpPut("{id}")]
    [Description("Edits an existing patron")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EditPatron([FromRoute] int id, [FromBody] EditPatronCommand command)
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
    [Description("Deletes a patron")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePatron([FromRoute] int id)
    {
        try
        {
            var command = new DeletePatronCommand(id);
            await _mediator.Send(command);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
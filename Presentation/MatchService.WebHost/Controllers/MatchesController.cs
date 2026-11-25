using MatchService.Application.Models.Match;
using MatchService.Application.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace MatchService.WebHost.Controllers;

[ApiController]
[Route("api/matches")]
public sealed class MatchesController(IMatchApplicationService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<MatchModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<MatchModel>>> GetAll(CancellationToken cancellationToken)
        => Ok(await service.GetModelsAsync(cancellationToken));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(MatchModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MatchModel>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var match = await service.GetModelByIdAsync(id, cancellationToken);
        return match is null ? NotFound() : Ok(match);
    }

    [HttpPost]
    [ProducesResponseType(typeof(MatchModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MatchModel>> Create(CreateMatchModel model, CancellationToken cancellationToken)
    {
        var match = await service.CreateModelAsync(model, cancellationToken);
        if (match is null)
            return BadRequest();

        return CreatedAtAction(nameof(GetById), new { id = match.Id }, match);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        => await service.DeleteModelAsync(id, cancellationToken) ? NoContent() : NotFound();
}

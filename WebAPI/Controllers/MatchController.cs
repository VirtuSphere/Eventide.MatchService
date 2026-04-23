using Eventide.MatchService.Application.Commands.CreateMatch;
using Eventide.MatchService.Application.Commands.SubmitResult;
using Eventide.MatchService.Application.Queries.GetMatch;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Eventide.MatchService.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MatchController : ControllerBase
{
    private readonly IMediator _mediator;

    public MatchController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMatchCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
    }

    [HttpPut("{id}/result")]
    public async Task<IActionResult> SubmitResult(Guid id, [FromBody] SubmitResultCommand command)
    {
        command = new SubmitResultCommand 
        { 
            MatchId = id, 
            WinnerId = command.WinnerId, 
            Player1Score = command.Player1Score, 
            Player2Score = command.Player2Score 
        };
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok() : BadRequest(result.ErrorMessage);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetMatch(Guid id)
    {
        var result = await _mediator.Send(new GetMatchQuery { MatchId = id });
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.ErrorMessage);
    }
}
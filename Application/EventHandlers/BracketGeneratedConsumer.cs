using Eventide.BracketService.Contracts.Events;
using Eventide.MatchService.Application.Commands.CreateMatch;
using MassTransit;
using MediatR;

namespace Eventide.MatchService.Application.EventHandlers;

public class BracketGeneratedConsumer : IConsumer<BracketGeneratedEvent>
{
    private readonly IMediator _mediator;

    public BracketGeneratedConsumer(IMediator mediator) => _mediator = mediator;

    public async Task Consume(ConsumeContext<BracketGeneratedEvent> context)
    {
        var msg = context.Message;
        
        foreach (var match in msg.Matches)
        {
            await _mediator.Send(new CreateMatchCommand
            {
                TournamentId = msg.TournamentId,
                BracketId = msg.BracketId,
                Player1Id = match.Player1Id,
                Player2Id = match.Player2Id
            });
        }
    }
}
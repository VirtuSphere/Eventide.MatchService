using Eventide.MatchService.Application.Common;
using MediatR;

namespace Eventide.MatchService.Application.Commands.CreateMatch;

public class CreateMatchCommand : IRequest<Result<Guid>>
{
    public Guid TournamentId { get; init; }
    public Guid BracketId { get; init; }
    public Guid Player1Id { get; init; }
    public Guid Player2Id { get; init; }
}
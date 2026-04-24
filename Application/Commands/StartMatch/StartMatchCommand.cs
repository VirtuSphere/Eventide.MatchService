using Eventide.MatchService.Application.Common;
using MediatR;

namespace Eventide.MatchService.Application.Commands.StartMatch;

public class StartMatchCommand : IRequest<Result>
{
    public Guid MatchId { get; init; }
}
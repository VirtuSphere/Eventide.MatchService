using Eventide.MatchService.Application.Common;
using MediatR;

namespace Eventide.MatchService.Application.Commands.SubmitResult;

public class SubmitResultCommand : IRequest<Result>
{
    public Guid MatchId { get; init; }
    public Guid WinnerId { get; init; }
    public int Player1Score { get; init; }
    public int Player2Score { get; init; }
}
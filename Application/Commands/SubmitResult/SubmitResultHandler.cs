using Eventide.MatchService.Application.Common;
using Eventide.MatchService.Contracts.Events;
using Eventide.MatchService.Domain.Interfaces;
using MassTransit;
using MediatR;

namespace Eventide.MatchService.Application.Commands.SubmitResult;

public class SubmitResultHandler : IRequestHandler<SubmitResultCommand, Result>
{
    private readonly IMatchRepository _repo;
    private readonly IPublishEndpoint _publishEndpoint;

    public SubmitResultHandler(IMatchRepository repo, IPublishEndpoint publishEndpoint)
    {
        _repo = repo;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result> Handle(SubmitResultCommand req, CancellationToken ct)
    {
        var match = await _repo.GetByIdAsync(req.MatchId, ct);
        if (match is null) return Result.Failure("Match not found");

        match.SubmitResult(req.WinnerId, req.Player1Score, req.Player2Score);
        await _repo.SaveChangesAsync(ct);

        var loserId = req.WinnerId == match.Player1Id ? match.Player2Id : match.Player1Id;

        await _publishEndpoint.Publish(new MatchCompletedEvent
        {
            MatchId = match.Id,
            WinnerId = req.WinnerId,
            LoserId = loserId,
            WinnerScore = req.Player1Score,
            LoserScore = req.Player2Score
        }, ct);

        return Result.Success();
    }
}
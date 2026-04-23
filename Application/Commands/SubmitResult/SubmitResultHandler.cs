using Eventide.MatchService.Application.Common;
using Eventide.MatchService.Domain.Interfaces;
using MediatR;

namespace Eventide.MatchService.Application.Commands.SubmitResult;

public class SubmitResultHandler : IRequestHandler<SubmitResultCommand, Result>
{
    private readonly IMatchRepository _repo;

    public SubmitResultHandler(IMatchRepository repo) => _repo = repo;

    public async Task<Result> Handle(SubmitResultCommand req, CancellationToken ct)
    {
        var match = await _repo.GetByIdAsync(req.MatchId, ct);
        if (match is null) return Result.Failure("Match not found");

        match.SubmitResult(req.WinnerId, req.Player1Score, req.Player2Score);
        await _repo.SaveChangesAsync(ct);

        return Result.Success();
    }
}
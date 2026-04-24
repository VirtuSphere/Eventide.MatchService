using Eventide.MatchService.Application.Common;
using Eventide.MatchService.Domain.Interfaces;
using MediatR;

namespace Eventide.MatchService.Application.Commands.StartMatch;

public class StartMatchHandler : IRequestHandler<StartMatchCommand, Result>
{
    private readonly IMatchRepository _repo;

    public StartMatchHandler(IMatchRepository repo) => _repo = repo;

    public async Task<Result> Handle(StartMatchCommand req, CancellationToken ct)
    {
        var match = await _repo.GetByIdAsync(req.MatchId, ct);
        if (match is null) return Result.Failure("Match not found");

        match.Start("localhost", "de_dust2");
        await _repo.SaveChangesAsync(ct);
        return Result.Success();
    }
}
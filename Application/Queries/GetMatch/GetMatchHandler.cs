using Eventide.MatchService.Application.Common;
using Eventide.MatchService.Application.DTOs;
using Eventide.MatchService.Domain.Interfaces;
using MediatR;

namespace Eventide.MatchService.Application.Queries.GetMatch;

public class GetMatchHandler : IRequestHandler<GetMatchQuery, Result<MatchDto>>
{
    private readonly IMatchRepository _repo;

    public GetMatchHandler(IMatchRepository repo) => _repo = repo;

    public async Task<Result<MatchDto>> Handle(GetMatchQuery req, CancellationToken ct)
    {
        var match = await _repo.GetByIdAsync(req.MatchId, ct);
        if (match is null) return Result<MatchDto>.Failure("Match not found");

        return Result<MatchDto>.Success(new MatchDto
        {
            Id = match.Id,
            TournamentId = match.TournamentId,
            Player1Id = match.Player1Id,
            Player2Id = match.Player2Id,
            WinnerId = match.WinnerId,
            Status = match.Status.ToString(),
            Player1Score = match.Player1Score,
            Player2Score = match.Player2Score,
            ScheduledTime = match.ScheduledTime,
            MapName = match.MapName
        });
    }
}
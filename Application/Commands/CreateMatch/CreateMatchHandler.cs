using Eventide.MatchService.Application.Common;
using Eventide.MatchService.Domain.Entities;
using Eventide.MatchService.Domain.Interfaces;
using MediatR;

namespace Eventide.MatchService.Application.Commands.CreateMatch;

public class CreateMatchHandler : IRequestHandler<CreateMatchCommand, Result<Guid>>
{
    private readonly IMatchRepository _repo;

    public CreateMatchHandler(IMatchRepository repo) => _repo = repo;

    public async Task<Result<Guid>> Handle(CreateMatchCommand req, CancellationToken ct)
    {
        var match = Match.Create(req.TournamentId, req.BracketId, req.Player1Id, req.Player2Id);
        await _repo.AddAsync(match, ct);
        await _repo.SaveChangesAsync(ct);

        return Result<Guid>.Success(match.Id);
    }
}
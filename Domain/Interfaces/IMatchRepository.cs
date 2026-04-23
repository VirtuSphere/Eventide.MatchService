using Eventide.MatchService.Domain.Entities;

namespace Eventide.MatchService.Domain.Interfaces;

public interface IMatchRepository
{
    Task<Match?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<Match>> GetByTournamentIdAsync(Guid tournamentId, CancellationToken ct = default);
    Task<List<Match>> GetByPlayerIdAsync(Guid playerId, CancellationToken ct = default);
    Task AddAsync(Match match, CancellationToken ct = default);
    Task UpdateAsync(Match match, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
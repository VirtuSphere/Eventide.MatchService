using Eventide.MatchService.Domain.Entities;
using Eventide.MatchService.Domain.Interfaces;
using Eventide.MatchService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Eventide.MatchService.Infrastructure.Repositories;

public class MatchRepository : IMatchRepository
{
    private readonly MatchDbContext _context;

    public MatchRepository(MatchDbContext context) => _context = context;

    public async Task<Match?> GetByIdAsync(Guid id, CancellationToken ct)
        => await _context.Matches.FindAsync(new object[] { id }, ct);

    public async Task<List<Match>> GetByTournamentIdAsync(Guid tournamentId, CancellationToken ct)
        => await _context.Matches.Where(m => m.TournamentId == tournamentId).ToListAsync(ct);

    public async Task<List<Match>> GetByPlayerIdAsync(Guid playerId, CancellationToken ct)
        => await _context.Matches.Where(m => m.Player1Id == playerId || m.Player2Id == playerId).ToListAsync(ct);

    public async Task AddAsync(Match match, CancellationToken ct)
        => await _context.Matches.AddAsync(match, ct);

    public Task UpdateAsync(Match match, CancellationToken ct)
    { _context.Matches.Update(match); return Task.CompletedTask; }

    public async Task SaveChangesAsync(CancellationToken ct) => await _context.SaveChangesAsync(ct);
}
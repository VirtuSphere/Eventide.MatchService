using MatchService.Domain;
using MatchService.Domain.Repositories.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MatchService.Infrastructure.EntityFramework.RepositoryEF;

public sealed class EfMatchRepository : EfRepository<Match, Guid>, IMatchRepository
{
    private readonly ApplicationDbContext _context;

    public EfMatchRepository(ApplicationDbContext context)
        : base(context)
    {
        _context = context;
    }

    public override async Task<IReadOnlyCollection<Match>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Matches
            .Include(match => match.Tournament)
            .Include(match => match.Bracket)
            .Include(match => match.Administrator)
            .Include(match => match.Player1)
            .Include(match => match.Player2)
            .Include(match => match.Winner)
            .ToListAsync(cancellationToken);

    public override async Task<Match?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Matches
            .Include(match => match.Tournament)
            .Include(match => match.Bracket)
            .Include(match => match.Administrator)
            .Include(match => match.Player1)
            .Include(match => match.Player2)
            .Include(match => match.Winner)
            .FirstOrDefaultAsync(match => match.Id == id, cancellationToken);
}

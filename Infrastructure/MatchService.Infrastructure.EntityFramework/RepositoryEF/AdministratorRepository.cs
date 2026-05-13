using Microsoft.EntityFrameworkCore;
using MatchService.Domain;
using MatchService.Domain.Repositories.Abstractions.Repositories;
using MatchService.Infrastructure.EntityFramework;

namespace MatchService.Infrastructure.EntityFramework.RepositoryEF;

public class EfAdministratorRepository(ApplicationDbContext context)
    : EfRepository<Administrator, Guid>(context), IAdministratorRepository
{
    private readonly DbSet<Administrator> _administrators = context.Set<Administrator>();

    public async Task<Administrator?> GetAdministratorByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username, nameof(username));
        return await _administrators
            .FirstOrDefaultAsync(a => a.Username.Value == username, cancellationToken);
    }

    public async Task<Administrator?> GetAdministratorByMatchIdAsync(Guid matchId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(matchId.ToString(), nameof(matchId));
        return await _administrators
            .Include(a => a.Matches)
            .FirstOrDefaultAsync(a => a.Matches.Any(m => m.Id == matchId), cancellationToken);
    }
}
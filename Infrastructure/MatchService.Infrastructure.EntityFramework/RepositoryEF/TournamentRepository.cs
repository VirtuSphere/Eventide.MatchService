using MatchService.Domain;
using MatchService.Domain.Repositories.Abstractions.Repositories;

namespace MatchService.Infrastructure.EntityFramework.RepositoryEF;

public sealed class EfTournamentRepository(ApplicationDbContext context)
    : EfRepository<Tournament, Guid>(context), ITournamentRepository
{
}

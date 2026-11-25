using MatchService.Domain;
using MatchService.Domain.Repositories.Abstractions.Repositories;

namespace MatchService.Infrastructure.EntityFramework.RepositoryEF;

public sealed class EfBracketRepository(ApplicationDbContext context)
    : EfRepository<Bracket, Guid>(context), IBracketRepository
{
}

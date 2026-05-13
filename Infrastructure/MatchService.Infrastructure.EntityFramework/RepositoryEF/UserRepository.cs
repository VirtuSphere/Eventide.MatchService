using MatchService.Domain;
using MatchService.Domain.Repositories.Abstractions.Repositories;
using MatchService.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace MatchService.Infrastructure.EntityFramework.RepositoryEF;

public class EfUserRepository(ApplicationDbContext context)
    : EfRepository<User, Guid>(context), IUserRepository
{
    private readonly DbSet<User> _users = context.Set<User>();

    public async Task<User?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username, nameof(username));
        
        return await _users
            .FirstOrDefaultAsync(u => u.Username.Value == username, cancellationToken);
    }
}

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MatchService.Infrastructure.EntityFramework;
using MatchService.Infrastructure.EntityFramework.RepositoryEF;
using Xunit;

namespace MatchService.Tests;

public sealed class AdministratorRepositoryTests : IDisposable
{
    private readonly SqliteConnection connection = new("Data Source=:memory:");
    private readonly ApplicationDbContext context;

    public AdministratorRepositoryTests()
    {
        connection.Open();
        context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options);
        context.Database.EnsureCreated();
    }

    [Fact]
    public async Task GetAdministratorByUsername_TranslatesValueObjectQuery()
    {
        var expected = TestData.Administrator("LookupAdmin");
        context.Administrators.Add(expected);
        await context.SaveChangesAsync();

        var repository = new EfAdministratorRepository(context);
        var result = await repository.GetAdministratorByUsernameAsync("LookupAdmin", CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(expected.Id, result.Id);
    }

    [Fact]
    public async Task BaseRepository_SupportsCrudAndExists()
    {
        var repository = new EfRepository<MatchService.Domain.Administrator, Guid>(context);
        var administrator = TestData.Administrator();

        await repository.AddAsync(administrator);
        Assert.True(await repository.ExistsAsync(administrator.Id));
        Assert.Single(await repository.GetAllAsync());
        Assert.NotNull(await repository.GetByIdAsync(administrator.Id));

        administrator.ChangeUsername(new MatchService.ValueObjects.Username("Updated"));
        await repository.UpdateAsync(administrator);
        await repository.DeleteAsync(administrator);

        Assert.False(await repository.ExistsAsync(administrator.Id));
    }

    public void Dispose()
    {
        context.Dispose();
        connection.Dispose();
    }
}

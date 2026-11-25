using MatchService.Domain;
using MatchService.Infrastructure.EntityFramework;
using MatchService.Infrastructure.EntityFramework.RepositoryEF;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MatchService.Tests;

public sealed class MatchRepositoryTests : IDisposable
{
    private readonly SqliteConnection connection = new("Data Source=:memory:");
    private readonly ApplicationDbContext context;

    public MatchRepositoryTests()
    {
        connection.Open();
        context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options);
        context.Database.EnsureCreated();
    }

    [Fact]
    public async Task MatchRepositoryLoadsRequiredNavigationsForListAndDetails()
    {
        var match = TestData.Match();
        match.Start(new MatchService.ValueObjects.ServerInfo("server"), new MatchService.ValueObjects.MapName("Ancient"));
        match.SubmitResult(match.Player1, new MatchService.ValueObjects.PlayerScore(1),
            new MatchService.ValueObjects.PlayerScore(0));
        context.Matches.Add(match);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
        var repository = new EfMatchRepository(context);

        var listedMatch = Assert.Single(await repository.GetAllAsync());
        Assert.NotNull(listedMatch.Tournament);
        Assert.NotNull(listedMatch.Bracket);
        Assert.NotNull(listedMatch.Administrator);
        Assert.NotNull(listedMatch.Player1);
        Assert.NotNull(listedMatch.Player2);
        Assert.NotNull(listedMatch.Winner);

        context.ChangeTracker.Clear();
        var loadedMatch = await repository.GetByIdAsync(match.Id);
        Assert.NotNull(loadedMatch);
        Assert.NotNull(loadedMatch.Tournament);
        Assert.NotNull(loadedMatch.Bracket);
        Assert.NotNull(loadedMatch.Administrator);
        Assert.NotNull(loadedMatch.Player1);
        Assert.NotNull(loadedMatch.Player2);
        Assert.NotNull(loadedMatch.Winner);
        Assert.Null(await repository.GetByIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task BaseRepositoryDeleteByIdReturnsFalseForMissingAndDeletesExisting()
    {
        var repository = new EfRepository<Administrator, Guid>(context);
        var administrator = TestData.Administrator();
        await repository.AddAsync(administrator);

        Assert.False(await repository.DeleteAsync(Guid.NewGuid()));
        Assert.True(await repository.DeleteAsync(administrator.Id));
        Assert.False(await repository.ExistsAsync(administrator.Id));
    }

    [Fact]
    public void EmptyEntityRepositoriesCanBeConstructed()
    {
        Assert.NotNull(new EfBracketRepository(context));
        Assert.NotNull(new EfTournamentRepository(context));
    }

    public void Dispose()
    {
        context.Dispose();
        connection.Dispose();
    }
}

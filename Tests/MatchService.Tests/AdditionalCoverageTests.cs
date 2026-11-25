using AutoMapper;
using MatchService.Application.Models.Administrator;
using MatchService.Application.Services.Mapping;
using MatchService.Domain;
using MatchService.Domain.Enums;
using MatchService.Domain.Exceptions;
using MatchService.Infrastructure.EntityFramework;
using MatchService.Infrastructure.EntityFramework.RepositoryEF;
using MatchService.ValueObjects;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MatchService.Tests;

public sealed class AdditionalDomainCoverageTests
{
    [Fact]
    public void Match_RejectsInvalidStateTransitions()
    {
        var match = TestData.Match();
        var player = match.Player1;

        Assert.Throws<MatchNotInProgressException>(() =>
            match.SubmitResult(player, new PlayerScore(1), new PlayerScore(0)));
        Assert.Throws<MatchNotInProgressException>(() =>
            match.Edit(mapName: new MapName("Ancient")));

        match.Start(new ServerInfo("server"), new MapName("Ancient"));
        Assert.Throws<MatchNotScheduledException>(() => match.Schedule(DateTime.UtcNow));
        Assert.Throws<MatchNotScheduledException>(() =>
            match.Edit(scheduledTime: DateTime.UtcNow));
        Assert.Throws<MatchWinnerNotParticipantException>(() =>
            match.Edit(winner: TestData.User("Outside")));
        Assert.Throws<MatchNotScheduledException>(() =>
            match.Start(new ServerInfo("server"), new MapName("Ancient")));
    }

    [Fact]
    public void Match_DisputeRequiresParticipantAndCompletedStatus()
    {
        var match = TestData.Match();
        Assert.Throws<InvalidMatchStatusException>(() => TestData.User().DisputeMatch(match));

        match.Start(new ServerInfo("server"), new MapName("Ancient"));
        match.SubmitResult(match.Player1, new PlayerScore(1), new PlayerScore(0));
        Assert.Throws<MatchWinnerNotParticipantException>(() => match.Dispute(TestData.User("Outside")));
        Assert.True(match.Player1.DisputeMatch(match));
        Assert.Equal(MatchStatus.Disputed, match.Status);
    }

    [Fact]
    public void Administrator_WrapperMethodsRejectNullAndUnknownMatches()
    {
        var administrator = TestData.Administrator();
        var unknown = TestData.Match();

        Assert.Throws<ArgumentNullValueException>(() => administrator.EditMatch(null!));
        Assert.Throws<ArgumentNullValueException>(() => administrator.DeleteMatch(null!));
        Assert.Throws<ArgumentNullValueException>(() => administrator.RescheduleMatch(null!, DateTime.UtcNow));
        Assert.Throws<ArgumentNullValueException>(() => administrator.CancelMatch(null!));
        Assert.Throws<ArgumentNullValueException>(() => administrator.DisputeMatch(null!));
        Assert.Throws<ArgumentNullValueException>(() =>
            administrator.StartMatch(null!, new ServerInfo("server"), new MapName("Ancient")));
        Assert.Throws<ArgumentNullValueException>(() =>
            administrator.SubmitMatchResult(null!, TestData.User(), new PlayerScore(1), new PlayerScore(0)));

        Assert.Throws<MatchNotFoundException>(() => administrator.EditMatch(unknown));
        Assert.Throws<MatchNotFoundException>(() => administrator.RescheduleMatch(unknown, DateTime.UtcNow));
        Assert.Throws<MatchNotFoundException>(() => administrator.CancelMatch(unknown));
        Assert.Throws<MatchNotFoundException>(() => administrator.DisputeMatch(unknown));
        Assert.Throws<MatchNotFoundException>(() =>
            administrator.StartMatch(unknown, new ServerInfo("server"), new MapName("Ancient")));
    }

    [Fact]
    public void ApplicationProfile_MapsMatchAndOptionalValues()
    {
        var mapper = new MapperConfiguration(
            configuration => configuration.AddProfile<ApplicationProfile>()).CreateMapper();
        var administrator = TestData.Administrator();
        var match = administrator.CreateMatch(
            new MatchService.Domain.Tournament(Guid.NewGuid()),
            new MatchService.Domain.Bracket(Guid.NewGuid()),
            TestData.User(),
            TestData.User("PlayerTwo"));

        var result = mapper.Map<MatchService.Application.Models.Match.MatchModel>(match);

        Assert.Equal(match.Tournament.Id, result.TournamentId);
        Assert.Equal(match.Player1.Id, result.Player1Id);
        Assert.Null(result.WinnerId);
        Assert.Null(result.MapName);
    }

    [Fact]
    public void Match_EditAndDisputeCoverIdempotentAndNullBranches()
    {
        var match = TestData.Match();
        var player1 = match.Player1;

        Assert.False(match.Edit());
        Assert.Throws<ArgumentNullValueException>(() =>
            match.Start(null!, new MapName("Ancient")));
        Assert.Throws<ArgumentNullValueException>(() =>
            match.Start(new ServerInfo("server"), null!));

        Assert.True(match.Start(new ServerInfo("server"), new MapName("Ancient")));
        Assert.False(match.Edit(mapName: new MapName("Ancient")));
        Assert.False(match.Edit(serverInfo: new ServerInfo("server")));
        Assert.True(match.Edit(winner: player1));
        Assert.False(match.Edit(winner: player1));

        Assert.True(match.SubmitResult(player1, new PlayerScore(1), new PlayerScore(0)));
        Assert.True(match.Dispute(player1));
        Assert.False(match.Dispute(player1));
        Assert.Contains("Match:", match.ToString());
    }

    [Fact]
    public void Match_SubmitResultRejectsNullArguments()
    {
        var match = TestData.Match();
        match.Start(new ServerInfo("server"), new MapName("Ancient"));

        Assert.Throws<ArgumentNullValueException>(() =>
            match.SubmitResult(null!, new PlayerScore(1), new PlayerScore(0)));
        Assert.Throws<ArgumentNullValueException>(() =>
            match.SubmitResult(match.Player1, null!, new PlayerScore(0)));
        Assert.Throws<ArgumentNullValueException>(() =>
            match.SubmitResult(match.Player1, new PlayerScore(1), null!));
    }

    [Fact]
    public void DomainExceptionsExposeMessages()
    {
        var exception = new InvalidMatchWinnerException(
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            "Winner",
            "Actual");

        Assert.Contains("11111111-1111-1111-1111-111111111111", exception.Message);
        Assert.Contains("Winner", exception.Message);
        Assert.Contains("Actual", exception.Message);
    }
}

public sealed class AdditionalRepositoryCoverageTests : IDisposable
{
    private readonly SqliteConnection connection = new("Data Source=:memory:");
    private readonly ApplicationDbContext context;

    public AdditionalRepositoryCoverageTests()
    {
        connection.Open();
        context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options);
        context.Database.EnsureCreated();
    }

    [Fact]
    public async Task Repositories_FindUsersAndAdministratorsByRelations()
    {
        var administrator = TestData.Administrator("LookupAdmin");
        var user = TestData.User("LookupUser");
        context.Administrators.Add(administrator);
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var userRepository = new EfUserRepository(context);
        Assert.Equal(user.Id, (await userRepository.GetUserByUsernameAsync("LookupUser", CancellationToken.None))!.Id);

        var match = administrator.CreateMatch(
            new MatchService.Domain.Tournament(Guid.NewGuid()),
            new MatchService.Domain.Bracket(Guid.NewGuid()),
            user,
            TestData.User("SecondUser"));
        context.Matches.Add(match);
        await context.SaveChangesAsync();

        var administratorRepository = new EfAdministratorRepository(context);
        var result = await administratorRepository.GetAdministratorByMatchIdAsync(match.Id, CancellationToken.None);
        Assert.Equal(administrator.Id, result!.Id);
    }

    [Fact]
    public async Task Repositories_RejectInvalidUsernames()
    {
        var administratorRepository = new EfAdministratorRepository(context);
        var userRepository = new EfUserRepository(context);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            administratorRepository.GetAdministratorByUsernameAsync(" ", CancellationToken.None));
        await Assert.ThrowsAsync<ArgumentException>(() =>
            userRepository.GetUserByUsernameAsync(" ", CancellationToken.None));
    }

    public void Dispose()
    {
        context.Dispose();
        connection.Dispose();
    }
}

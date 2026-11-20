using MatchService.Domain.Enums;
using MatchService.Domain.Exceptions;
using MatchService.ValueObjects;
using Xunit;

namespace MatchService.Tests;

public sealed class MatchTests
{
    [Fact]
    public void NewMatch_IsScheduled()
    {
        var match = TestData.Match();

        Assert.Equal(MatchStatus.Scheduled, match.Status);
        Assert.Null(match.ScheduledTime);
    }

    [Fact]
    public void Schedule_ChangesTimeOnce()
    {
        var match = TestData.Match();
        var time = DateTime.UtcNow.AddHours(1);

        Assert.True(match.Schedule(time));
        Assert.False(match.Schedule(time));
        Assert.Equal(time, match.ScheduledTime);
    }

    [Fact]
    public void Start_RequiresScheduledStatusAndSetsServerData()
    {
        var match = TestData.Match();

        Assert.True(match.Start(new ServerInfo("server-1"), new MapName("Ancient")));
        Assert.Equal(MatchStatus.InProgress, match.Status);
        Assert.NotNull(match.StartedAt);
        Assert.Equal("server-1", match.ServerInfo!.Value);
    }

    [Fact]
    public void SubmitResult_CompletesMatch()
    {
        var match = TestData.Match();
        match.Start(new ServerInfo("server-1"), new MapName("Ancient"));

        Assert.True(match.SubmitResult(match.Player1, new PlayerScore(3), new PlayerScore(1)));
        Assert.Equal(MatchStatus.Completed, match.Status);
        Assert.Equal(match.Player1, match.Winner);
        Assert.Equal(3, match.Player1Score!.Value);
    }

    [Fact]
    public void SubmitResult_RejectsNonParticipant()
    {
        var match = TestData.Match();
        match.Start(new ServerInfo("server-1"), new MapName("Ancient"));

        Assert.Throws<MatchWinnerNotParticipantException>(() =>
            match.SubmitResult(TestData.User("Outside"), new PlayerScore(1), new PlayerScore(0)));
    }

    [Fact]
    public void Edit_UpdatesAllowedFieldsForCurrentStatus()
    {
        var match = TestData.Match();
        var scheduledTime = DateTime.UtcNow.AddMinutes(30);

        Assert.True(match.Edit(scheduledTime: scheduledTime));
        match.Start(new ServerInfo("server-1"), new MapName("Ancient"));
        Assert.True(match.Edit(mapName: new MapName("Inferno"), serverInfo: new ServerInfo("server-2")));
        Assert.Equal("Inferno", match.MapName!.Value);
    }

    [Fact]
    public void DisputeAndCancel_AreIdempotent()
    {
        var disputed = TestData.Match();
        disputed.Start(new ServerInfo("server-1"), new MapName("Ancient"));
        disputed.SubmitResult(disputed.Player1, new PlayerScore(1), new PlayerScore(0));

        Assert.True(disputed.Dispute());
        Assert.False(disputed.Dispute());

        var cancelled = TestData.Match();
        Assert.True(cancelled.Cancel());
        Assert.False(cancelled.Cancel());
        Assert.Equal(MatchStatus.Cancelled, cancelled.Status);
    }

    [Fact]
    public void Match_RejectsSamePlayerTwice()
    {
        var administrator = TestData.Administrator();
        var player = TestData.User();

        Assert.Throws<MatchPlayerSameException>(() =>
            administrator.CreateMatch(
                new MatchService.Domain.Tournament(Guid.NewGuid()),
                new MatchService.Domain.Bracket(Guid.NewGuid()),
                player,
                player));
    }
}

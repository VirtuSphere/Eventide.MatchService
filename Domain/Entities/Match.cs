using Eventide.MatchService.Domain.Enums;
using Eventide.MatchService.Domain.Exceptions;

namespace Eventide.MatchService.Domain.Entities;

public class Match
{
    public Guid Id { get; private set; }
    public Guid TournamentId { get; private set; }
    public Guid BracketId { get; private set; }
    public Guid Player1Id { get; private set; }
    public Guid Player2Id { get; private set; }
    public Guid? WinnerId { get; private set; }
    public MatchStatus Status { get; private set; }
    public DateTime? ScheduledTime { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? MapName { get; private set; }
    public string? ServerInfo { get; private set; }
    public int Player1Score { get; private set; }
    public int Player2Score { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Match() { }

    public static Match Create(Guid tournamentId, Guid bracketId, Guid player1Id, Guid player2Id)
    {
        if (player1Id == player2Id) throw new DomainException("Player cannot play against themselves");

        return new Match
        {
            Id = Guid.NewGuid(),
            TournamentId = tournamentId,
            BracketId = bracketId,
            Player1Id = player1Id,
            Player2Id = player2Id,
            Status = MatchStatus.Scheduled,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Schedule(DateTime time)
    {
        if (Status != MatchStatus.Scheduled) throw new DomainException("Match is not in scheduled status");
        ScheduledTime = time;
    }

    public void Start(string serverInfo, string mapName)
    {
        if (Status != MatchStatus.Scheduled) throw new DomainException("Match not scheduled");
        Status = MatchStatus.InProgress;
        StartedAt = DateTime.UtcNow;
        ServerInfo = serverInfo;
        MapName = mapName;
    }

    public void SubmitResult(Guid winnerId, int player1Score, int player2Score)
    {
        if (Status != MatchStatus.InProgress) throw new DomainException("Match not in progress");
        if (winnerId != Player1Id && winnerId != Player2Id) throw new DomainException("Winner not a participant");

        WinnerId = winnerId;
        Player1Score = player1Score;
        Player2Score = player2Score;
        Status = MatchStatus.Completed;
        CompletedAt = DateTime.UtcNow;
    }

    public void Dispute() => Status = MatchStatus.Disputed;
    public void Cancel() => Status = MatchStatus.Cancelled;
}
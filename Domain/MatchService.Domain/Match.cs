using MatchService.Domain.Base;
using MatchService.Domain.Enums;
using MatchService.Domain.Exceptions;
using MatchService.ValueObjects;

namespace MatchService.Domain;

public class Match : Entity<Guid>
{
    public Guid TournamentId { get; private set; }
    public Guid BracketId { get; private set; }
    public Administrator Administrator { get; private set; } = null!;
    public User Player1 { get; private set; } = null!;
    public User Player2 { get; private set; } = null!;
    public User? Winner { get; private set; }
    public MatchStatus Status { get; private set; }
    public DateTime? ScheduledTime { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public MapName? MapName { get; private set; }
    public ServerInfo? ServerInfo { get; private set; }
    public PlayerScore? Player1Score { get; private set; }
    public PlayerScore? Player2Score { get; private set; }
    public DateTime CreatedAt { get; private set; }

    protected Match()
    {
    }

    public Match(Guid tournamentId, Guid bracketId, Administrator administrator, User player1, User player2)
        : this(Guid.NewGuid(), tournamentId, bracketId, administrator, player1, player2, DateTime.UtcNow)
    {}

    protected Match(
        Guid id,
        Guid tournamentId,
        Guid bracketId,
        Administrator administrator,
        User player1,
        User player2,
        DateTime createdAt) : base(id)
    {
        administrator = administrator ?? throw new ArgumentNullValueException(nameof(administrator));
        player1 = player1 ?? throw new ArgumentNullValueException(nameof(player1));
        player2 = player2 ?? throw new ArgumentNullValueException(nameof(player2));

        if (player1.Id == player2.Id)
        {
            throw new MatchPlayerSameException();
        }
        TournamentId = tournamentId;
        BracketId = bracketId;
        Administrator = administrator;
        Player1 = player1;
        Player2 = player2;
        Status = MatchStatus.Scheduled;
        CreatedAt = createdAt;
    }
    /// <summary>
    /// Позволяет администратору установить победителя матча.
    /// </summary>
    /// <param name="winner"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullValueException"></exception>
    /// <exception cref="MatchNotInProgressException"></exception>
    /// <exception cref="MatchWinnerNotParticipantException"></exception>
    public bool SetWinner(User winner)
    {
        winner = winner ?? throw new ArgumentNullValueException(nameof(winner));

        if (Status != MatchStatus.InProgress)
        {
            throw new MatchNotInProgressException(Status);
        }

        if (winner != Player1 && winner != Player2)
        {
            throw new MatchWinnerNotParticipantException(winner, Player1, Player2);
        }

        if (Winner != null && Winner.Id == winner.Id)
        {
            return false;
        }
        Winner = winner;
        return true;
    }
    /// <summary>
    /// Позволяет администратору установить карту для матча.
    /// </summary>
    /// <param name="mapName"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullValueException"></exception>
    /// <exception cref="MatchNotInProgressException"></exception>
    public bool SetMap(MapName mapName)
    {
        mapName = mapName ?? throw new ArgumentNullValueException(nameof(mapName));

        if (Status != MatchStatus.InProgress)
        {
            throw new MatchNotInProgressException(Status);
        }

        if (MapName != null && MapName.Value == mapName.Value)
        {
            return false;
        }
        MapName = mapName;
        return true;
    }
    /// <summary>
    /// Позволяет администратору установить информацию о сервере для матча.
    /// </summary>
    /// <param name="serverInfo"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullValueException"></exception>
    /// <exception cref="MatchNotInProgressException"></exception>
    public bool SetServerInfo(ServerInfo serverInfo)
    {
        serverInfo = serverInfo ?? throw new ArgumentNullValueException(nameof(serverInfo));

        if (Status != MatchStatus.InProgress)
        {
            throw new MatchNotInProgressException(Status);
        }

        if (ServerInfo != null && ServerInfo.Equals(serverInfo))
        {
            return false;
        }
        ServerInfo = serverInfo;
        return true;
    }
    /// <summary>
    /// Позволяет администратору запланировать время начала матча.
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    /// <exception cref="MatchNotScheduledException"></exception>
    public bool Schedule(DateTime time)
    {
        if (Status != MatchStatus.Scheduled)
        {
            throw new MatchNotScheduledException(Status);
        }

        if (ScheduledTime == time)
        {
            return false;
        }

        ScheduledTime = time;
        return true;
    }
    /// <summary>
    /// Позволяет администратору начать матч, указав информацию о сервере и карте.
    /// </summary>
    /// <param name="serverInfo"></param>
    /// <param name="mapName"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullValueException"></exception>
    /// <exception cref="MatchNotScheduledException"></exception>
    public bool Start(ServerInfo serverInfo, MapName mapName)
    {
        serverInfo = serverInfo ?? throw new ArgumentNullValueException(nameof(serverInfo));
        mapName = mapName ?? throw new ArgumentNullValueException(nameof(mapName));

        if (Status != MatchStatus.Scheduled)
        {
            throw new MatchNotScheduledException(Status);
        }

        ServerInfo = serverInfo;
        MapName = mapName;
        Status = MatchStatus.InProgress;
        StartedAt = DateTime.UtcNow;
        return true;
    }

    /// <summary>
    /// Позволяет администратору отправить результат матча.
    /// </summary>
    /// <param name="winner"></param>
    /// <param name="player1Score"></param>
    /// <param name="player2Score"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullValueException"></exception>
    /// <exception cref="MatchNotInProgressException"></exception>
    /// <exception cref="MatchWinnerNotParticipantException"></exception>
    public bool SubmitResult(User winner, PlayerScore player1Score, PlayerScore player2Score)
    {
        winner = winner ?? throw new ArgumentNullValueException(nameof(winner));
        player1Score = player1Score ?? throw new ArgumentNullValueException(nameof(player1Score));
        player2Score = player2Score ?? throw new ArgumentNullValueException(nameof(player2Score));

        if (Status != MatchStatus.InProgress)
        {
            throw new MatchNotInProgressException(Status);
        }

        if (winner != Player1 && winner != Player2)
        {
            throw new MatchWinnerNotParticipantException(winner, Player1, Player2);
        }

        Winner = winner;
        Player1Score = player1Score;
        Player2Score = player2Score;
        Status = MatchStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        return true;
    }
    /// <summary>
    /// Позволяет оспорить результат матча.
    /// </summary>
    /// <returns></returns>
    public bool Dispute()
    {
        if (Status == MatchStatus.Disputed)
        {
            return false;
        }

        Status = MatchStatus.Disputed;
        return true;
    }
    /// <summary>
    /// Позволяет пользователю оспорить результат матча.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public bool Dispute(User user)
    {
        if (Status == MatchStatus.Disputed)
        {
            return false;
        }
        if (user != Player1 && user != Player2)
        {
            throw new MatchWinnerNotParticipantException(user, Player1, Player2);
        }

        Status = MatchStatus.Disputed;
        return true;
    }

    /// <summary>
    /// Позволяет администратору отменить матч.
    /// </summary>
    /// <returns></returns>
    public bool Cancel()
    {
        if (Status == MatchStatus.Cancelled)
        {
            return false;
        }

        Status = MatchStatus.Cancelled;
        return true;
    }

    public override string ToString()
        => $"Match: {Id}, TournamentId: {TournamentId}, BracketId: {BracketId}, Player1Id: {Player1.Id}, Player2Id: {Player2.Id}, WinnerId: {Winner?.Id}, Status: {Status}, ScheduledTime: {ScheduledTime}, StartedAt: {StartedAt}, CompletedAt: {CompletedAt}, MapName: {MapName}, ServerInfo: {ServerInfo}, Player1Score: {Player1Score}, Player2Score: {Player2Score}, CreatedAt: {CreatedAt}";
}
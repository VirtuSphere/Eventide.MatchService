using MatchService.Domain.Base;
using MatchService.Domain.Enums;
using MatchService.Domain.Exceptions;
using MatchService.ValueObjects;

namespace MatchService.Domain;

public class Administrator(Guid id, Username username) : Entity<Guid>(id)
{
    public Username Username { get; private set; } = username ?? throw new ArgumentNullValueException(nameof(username));
    private readonly ICollection<Match> _matches = [];

    public IReadOnlyCollection<Match> Matches => _matches.ToList().AsReadOnly();
    internal bool ChangeUsername(Username newUsername)
    {
        if (newUsername == null) throw new ArgumentNullValueException(nameof(newUsername));

        if (Username == newUsername) return false;

        Username = newUsername;
        return true;
    }
    public Match CreateMatch(Guid tournamentId, Guid bracketId, User player1, User player2)
    {
        if (player1 == null) throw new ArgumentNullValueException(nameof(player1));
        if (player2 == null) throw new ArgumentNullValueException(nameof(player2));
        var match = new Match(tournamentId, bracketId, this, player1, player2);
        _matches.Add(match);
        return match;
    }
    public bool EditMatch(Match match, User? winner = null, DateTime? scheduledTime = null, MapName? mapName = null, ServerInfo? serverInfo = null)
    {
        if (match == null) throw new ArgumentNullValueException(nameof(match));
        if (!_matches.Contains(match)) throw new MatchNotFoundException(match.Id);
        if (winner != null)
        {
            match.SetWinner(winner);
        }
        if (scheduledTime != null)
        {
            match.Schedule(scheduledTime.Value);
        }
        if (mapName != null)
        {
            match.SetMap(mapName);
        }
        if (serverInfo != null)
        {
            match.SetServerInfo(serverInfo);
        }
        return true;
    }
    public bool DeleteMatch(Match match)
    {
        if (match == null) throw new ArgumentNullValueException(nameof(match));
        if (!_matches.Contains(match)) throw new MatchNotFoundException(match.Id);
        return _matches.Remove(match);
    }
    public bool RescheduleMatch(Match match, DateTime newTime)
    {
        if (match == null) throw new ArgumentNullValueException(nameof(match));
        if (!_matches.Contains(match)) throw new MatchNotFoundException(match.Id);
        return match.Schedule(newTime);
    }
    public bool CancelMatch(Match match)
    {
        if (match == null) throw new ArgumentNullValueException(nameof(match));
        if (!_matches.Contains(match)) throw new MatchNotFoundException(match.Id);
        return match.Cancel();
    }
    public bool DisputeMatch(Match match)
    {
        if (match == null) throw new ArgumentNullValueException(nameof(match));
        if (!_matches.Contains(match)) throw new MatchNotFoundException(match.Id);
        return match.Dispute();
    }
    public bool StartMatch(Match match, ServerInfo serverInfo, MapName mapName)
    {
        if (match == null) throw new ArgumentNullValueException(nameof(match));
        if (!_matches.Contains(match)) throw new MatchNotFoundException(match.Id);
        return match.Start(serverInfo, mapName);
    }
     public bool SubmitMatchResult(Match match, User winner, PlayerScore player1Score, PlayerScore player2Score)
        {
        if (match == null) throw new ArgumentNullValueException(nameof(match));
        if (!_matches.Contains(match)) throw new MatchNotFoundException(match.Id);
        if (winner == null) throw new ArgumentNullValueException(nameof(winner));
        if (player1Score == null) throw new ArgumentNullValueException(nameof(player1Score));
        if (player2Score == null) throw new ArgumentNullValueException(nameof(player2Score));
        match.SubmitResult(winner, player1Score, player2Score);
        return true;
    }


}

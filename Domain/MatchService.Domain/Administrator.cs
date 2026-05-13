using MatchService.Domain.Base;
using MatchService.Domain.Enums;
using MatchService.Domain.Exceptions;
using MatchService.ValueObjects;
using System.Data;

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
    public Match CreateMatch(Tournament tournament, Bracket bracket, User player1, User player2)
    {
        if (tournament == null) throw new ArgumentNullValueException(nameof(tournament));
        if (bracket == null) throw new ArgumentNullValueException(nameof(bracket));
        if (player1 == null) throw new ArgumentNullValueException(nameof(player1));
        if (player2 == null) throw new ArgumentNullValueException(nameof(player2));
        var match = new Match(tournament, bracket, this, player1, player2);  
        
        if(_matches.Contains(match)) throw new MatchNotFoundException(match.Id);
        _matches.Add(match);
        return match;
    }
    public bool EditMatch(Match match, User? winner = null, DateTime? scheduledTime = null, MapName? mapName = null, ServerInfo? serverInfo = null)
    {
        if (match == null) throw new ArgumentNullValueException(nameof(match));
        if (!_matches.Contains(match)) throw new MatchNotFoundException(match.Id);
        return match.Edit(winner, scheduledTime, mapName, serverInfo);
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
    /// <summary>
    /// отмена матча может быть выполнена только если матч не начался, то есть его статус должен быть Scheduled. 
    /// После отмены статус матча меняется на Canceled
    /// </summary>
    /// <param name="match"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullValueException"></exception>
    /// <exception cref="MatchNotFoundException"></exception>
    public bool CancelMatch(Match match)
    {
        if (match == null) throw new ArgumentNullValueException(nameof(match));
        if (!_matches.Contains(match)) throw new MatchNotFoundException(match.Id);
        return match.Cancel();
    }
    /// <summary>
    /// оспорить результат матча может 
    /// </summary>
    /// <param name="match"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullValueException"></exception>
    /// <exception cref="MatchNotFoundException"></exception>
    public bool DisputeMatch(Match match)
    {
        if (match == null) throw new ArgumentNullValueException(nameof(match));
        if (!_matches.Contains(match)) throw new MatchNotFoundException(match.Id);
        return match.Dispute();
    }
    /// <summary>
    ///  матч должен быть в статусе Scheduled, иначе он не может быть начат. 
    ///  При старте матча должны быть указаны сервер и карта, на которых будет проходить матч. После успешного старта статус матча меняется на Started
    /// </summary>
    /// <param name="match"></param>
    /// <param name="serverInfo"></param>
    /// <param name="mapName"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullValueException"></exception>
    /// <exception cref="MatchNotFoundException"></exception>
    public bool StartMatch(Match match, ServerInfo serverInfo, MapName mapName)
    {
        if (match == null) throw new ArgumentNullValueException(nameof(match));
        if (!_matches.Contains(match)) throw new MatchNotFoundException(match.Id);
        return match.Start(serverInfo, mapName);
    }
    /// <summary>
    /// матч должен быть в статусе Started, иначе результат не может быть принят
    /// </summary>
    /// <param name="match"></param>
    /// <param name="winner"></param>
    /// <param name="player1Score"></param>
    /// <param name="player2Score"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullValueException"></exception>
    /// <exception cref="MatchNotFoundException"></exception>
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

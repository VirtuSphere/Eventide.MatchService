using MatchService.Domain.Base;
using MatchService.Domain.Enums;
using MatchService.Domain.Exceptions;
using MatchService.ValueObjects;

namespace MatchService.Domain;

public class User(Guid id, Username username) : Entity<Guid>(id)
{
    public Username Username { get; private set; } = username ?? throw new ArgumentNullValueException(nameof(username));
    /// <summary>
    /// Позволяет пользователю изменить свое имя пользователя.
    /// </summary>
    /// <param name="newUsername"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullValueException"></exception>
    internal bool ChangeUsername(Username newUsername)
    {
        if (newUsername == null) throw new ArgumentNullValueException(nameof(newUsername));

        if (Username == newUsername) return false;

        Username = newUsername;
        return true;
    }
    /// <summary>
    /// Позволяет игроку оспорить результат матча.
    /// </summary>
    /// <param name="match"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullValueException"></exception>
    /// <exception cref="InvalidMatchStatusException"></exception>
    public bool DisputeMatch(Match match) 
    {
        if (match == null) throw new ArgumentNullValueException(nameof(match));
        if (match.Status != MatchStatus.Completed) throw new InvalidMatchStatusException(match.Id, match.Status);
        match.Dispute(this);
        return true;
    }
}

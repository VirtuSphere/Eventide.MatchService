namespace MatchService.Domain.Exceptions;

public class MatchWinnerNotParticipantException : DomainException
{
    public MatchWinnerNotParticipantException(User winner, User player1, User player2)
        : base($"Winner '{winner.Id}' is not a participant. Allowed participants: '{player1.Id}' or '{player2.Id}'.")
    {
    }
}
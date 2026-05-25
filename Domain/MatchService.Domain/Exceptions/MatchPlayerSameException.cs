namespace MatchService.Domain.Exceptions;

public class MatchPlayerSameException : DomainException
{
    public MatchPlayerSameException()
        : base("Player cannot play against themselves.")
    {
    }
}
namespace MatchService.Domain.Exceptions;

public class MatchNotScheduledException : DomainException
{
    public MatchNotScheduledException(Enum currentStatus)
        : base($"Match is not in scheduled status. Current status: {currentStatus}.")
    {
    }
}
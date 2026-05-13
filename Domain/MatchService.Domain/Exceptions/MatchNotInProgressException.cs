namespace MatchService.Domain.Exceptions;

public class MatchNotInProgressException : DomainException
{
    public MatchNotInProgressException(Enum currentStatus)
        : base($"Match is not in progress. Current status: {currentStatus}.")
    {
    }
}
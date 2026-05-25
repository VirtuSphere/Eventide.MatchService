using MatchService.Domain.Enums;

namespace MatchService.Domain.Exceptions;

public class InvalidMatchStatusException : DomainException
{
    public InvalidMatchStatusException(Guid matchId, MatchStatus status)
        : base($"Match with ID '{matchId}' has invalid status '{status}' for this operation.")
    {
    }

}

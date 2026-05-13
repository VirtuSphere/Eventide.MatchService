using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchService.Domain.Exceptions;

public class InvalidMatchWinnerException : DomainException
{
    public InvalidMatchWinnerException(Guid matchId, string expectedWinner, string actualWinner)
        : base($"Match with ID '{matchId}' has invalid winner. Expected: '{expectedWinner}', Actual: '{actualWinner}'.")
    {
    }
}

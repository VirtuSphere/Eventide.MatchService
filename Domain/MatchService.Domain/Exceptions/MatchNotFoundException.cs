using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchService.Domain.Exceptions;

public class MatchNotFoundException : DomainException
{
    public MatchNotFoundException(Guid matchId)
        : base($"Match with ID '{matchId}' was not found.")
    {
    }
}

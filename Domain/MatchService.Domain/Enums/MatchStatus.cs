using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchService.Domain.Enums;

public enum MatchStatus
{
    Scheduled,
    InProgress,
    Completed,
    Disputed,
    Cancelled
}
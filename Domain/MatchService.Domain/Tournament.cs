using MatchService.Domain.Base;

namespace MatchService.Domain;

public class Tournament: Entity<Guid>
{
    protected Tournament() : base() { }
    public Tournament(Guid id) : base(id) { }
}

using MatchService.Domain.Base;
namespace MatchService.Domain;

public class Bracket : Entity<Guid>
{
    protected Bracket() : base() { }
    public Bracket(Guid id) : base(id) { }
}

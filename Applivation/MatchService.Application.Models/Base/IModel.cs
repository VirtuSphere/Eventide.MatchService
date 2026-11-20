namespace MatchService.Application.Models.Base;

public interface IModel<out TId>
    where TId : struct, IEquatable<TId>
{
    TId Id { get; }
}

using MatchService.Application.Models.User;
using MatchService.Application.Services.Abstractions.Base;

namespace MatchService.Application.Services.Abstractions;

public interface IUserApplicationService : IApplicationService<UserModel, CreateUserModel, Guid>
{
    Task<UserModel?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default);
}

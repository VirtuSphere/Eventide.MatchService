using AutoMapper;
using MatchService.Application.Models.User;
using MatchService.Application.Services.Abstractions;
using MatchService.Domain;
using MatchService.Domain.Repositories.Abstractions.Repositories;
using MatchService.ValueObjects;

namespace MatchService.Application.Services;

public sealed class UserApplicationService(
    IUserRepository repository,
    IMapper mapper) : IUserApplicationService
{
    public async Task<IEnumerable<UserModel>> GetModelsAsync(CancellationToken cancellationToken = default)
        => (await repository.GetAllAsync(cancellationToken)).Select(mapper.Map<UserModel>);

    public async Task<UserModel?> GetModelByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await repository.GetByIdAsync(id, cancellationToken);
        return user is null ? null : mapper.Map<UserModel>(user);
    }

    public async Task<UserModel?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        var user = await repository.GetUserByUsernameAsync(username, cancellationToken);
        return user is null ? null : mapper.Map<UserModel>(user);
    }

    public async Task<UserModel?> CreateModelAsync(CreateUserModel userInformation, CancellationToken cancellationToken = default)
    {
        if (await GetUserByUsernameAsync(userInformation.Username, cancellationToken) is not null)
            return null;

        var user = new User(Guid.NewGuid(), new Username(userInformation.Username));
        await repository.AddAsync(user, cancellationToken);
        return mapper.Map<UserModel>(user);
    }

    public async Task<bool> UpdateModelAsync(UserModel userInformation, CancellationToken cancellationToken = default)
    {
        var user = await repository.GetByIdAsync(userInformation.Id, cancellationToken);
        if (user is null)
            return false;

        var existing = await repository.GetUserByUsernameAsync(userInformation.Username, cancellationToken);
        if (existing is not null && existing.Id != user.Id)
            return false;

        if (!user.ChangeUsername(new Username(userInformation.Username)))
            return true;

        await repository.UpdateAsync(user, cancellationToken);
        return true;
    }

    public async Task<bool> DeleteModelAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await repository.GetByIdAsync(id, cancellationToken);
        if (user is null)
            return false;

        await repository.DeleteAsync(user, cancellationToken);
        return true;
    }
}

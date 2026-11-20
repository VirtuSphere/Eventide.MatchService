using AutoMapper;
using MatchService.Application.Models.Administrator;
using MatchService.Application.Services.Abstractions;
using MatchService.Domain;
using MatchService.Domain.Repositories.Abstractions.Repositories;
using MatchService.ValueObjects;

namespace MatchService.Application.Services;

public sealed class AdministratorApplicationService(
    IAdministratorRepository repository,
    IMapper mapper) : IAdministratorApplicationService
{
    public async Task<IEnumerable<AdministratorModel>> GetModelsAsync(CancellationToken cancellationToken = default)
        => (await repository.GetAllAsync(cancellationToken))
            .Select(mapper.Map<AdministratorModel>);

    public async Task<AdministratorModel?> GetModelByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var administrator = await repository.GetByIdAsync(id, cancellationToken);
        return administrator is null ? null : mapper.Map<AdministratorModel>(administrator);
    }

    public async Task<AdministratorModel?> GetAdministratorByUsernameAsync(
        string username,
        CancellationToken cancellationToken = default)
    {
        var administrator = await repository.GetAdministratorByUsernameAsync(username, cancellationToken);
        return administrator is null ? null : mapper.Map<AdministratorModel>(administrator);
    }

    public async Task<AdministratorModel?> CreateModelAsync(
        CreateAdministratorModel administratorInformation,
        CancellationToken cancellationToken = default)
    {
        if (await GetAdministratorByUsernameAsync(administratorInformation.Username, cancellationToken) is not null)
            return null;

        var administrator = new Administrator(
            Guid.NewGuid(),
            new Username(administratorInformation.Username));
        await repository.AddAsync(administrator, cancellationToken);
        return mapper.Map<AdministratorModel>(administrator);
    }

    public async Task<bool> UpdateModelAsync(
        AdministratorModel administratorInformation,
        CancellationToken cancellationToken = default)
    {
        var administrator = await repository.GetByIdAsync(administratorInformation.Id, cancellationToken);
        if (administrator is null)
            return false;

        var existing = await repository.GetAdministratorByUsernameAsync(
            administratorInformation.Username,
            cancellationToken);
        if (existing is not null && existing.Id != administrator.Id)
            return false;

        administrator.ChangeUsername(new Username(administratorInformation.Username));
        await repository.UpdateAsync(administrator, cancellationToken);
        return true;
    }

    public async Task<bool> DeleteModelAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var administrator = await repository.GetByIdAsync(id, cancellationToken);
        if (administrator is null)
            return false;

        await repository.DeleteAsync(administrator, cancellationToken);
        return true;
    }
}
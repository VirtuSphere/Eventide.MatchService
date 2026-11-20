using AutoMapper;
using MatchService.Application.Models.Administrator;
using MatchService.Application.Services;
using MatchService.Application.Services.Mapping;
using MatchService.Domain.Repositories.Abstractions.Repositories;
using Moq;
using Xunit;

namespace MatchService.Tests;

public sealed class AdministratorApplicationServiceTests
{
    private readonly Mock<IAdministratorRepository> repository = new();
    private readonly IMapper mapper = new MapperConfiguration(
        configuration => configuration.AddProfile<ApplicationProfile>()).CreateMapper();

    [Fact]
    public async Task GetModelsAsync_MapsAllAdministrators()
    {
        var administrators = new[] { TestData.Administrator(), TestData.Administrator("SecondAdmin") };
        repository.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(administrators);

        var result = (await CreateService().GetModelsAsync()).ToArray();

        Assert.Equal(2, result.Length);
        Assert.Equal("SecondAdmin", result[1].Username);
    }

    [Fact]
    public async Task GetByIdAndUsername_ReturnNullWhenMissing()
    {
        repository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((MatchService.Domain.Administrator?)null);
        repository.Setup(x => x.GetAdministratorByUsernameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((MatchService.Domain.Administrator?)null);

        Assert.Null(await CreateService().GetModelByIdAsync(Guid.NewGuid()));
        Assert.Null(await CreateService().GetAdministratorByUsernameAsync("AdminUser"));
    }

    [Fact]
    public async Task CreateModelAsync_ReturnsNullForDuplicateUsername()
    {
        repository.Setup(x => x.GetAdministratorByUsernameAsync("AdminUser", It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestData.Administrator());

        Assert.Null(await CreateService().CreateModelAsync(new CreateAdministratorModel("AdminUser")));
        repository.Verify(x => x.AddAsync(It.IsAny<MatchService.Domain.Administrator>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateModelAsync_AddsAndMapsAdministrator()
    {
        repository.Setup(x => x.GetAdministratorByUsernameAsync("NewAdmin", It.IsAny<CancellationToken>()))
            .ReturnsAsync((MatchService.Domain.Administrator?)null);

        var result = await CreateService().CreateModelAsync(new CreateAdministratorModel("NewAdmin"));

        Assert.NotNull(result);
        Assert.Equal("NewAdmin", result.Username);
        repository.Verify(x => x.AddAsync(It.IsAny<MatchService.Domain.Administrator>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateModelAsync_ReturnsFalseForMissingOrDuplicate()
    {
        repository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((MatchService.Domain.Administrator?)null);
        Assert.False(await CreateService().UpdateModelAsync(new AdministratorModel(Guid.NewGuid(), "NewAdmin")));

        var administrator = TestData.Administrator();
        repository.Setup(x => x.GetByIdAsync(administrator.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(administrator);
        repository.Setup(x => x.GetAdministratorByUsernameAsync("Taken", It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestData.Administrator("Taken"));

        Assert.False(await CreateService().UpdateModelAsync(new AdministratorModel(administrator.Id, "Taken")));
    }

    [Fact]
    public async Task UpdateAndDeleteModelAsync_ReturnTrueForExistingAdministrator()
    {
        var administrator = TestData.Administrator();
        repository.Setup(x => x.GetByIdAsync(administrator.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(administrator);
        repository.Setup(x => x.GetAdministratorByUsernameAsync("Updated", It.IsAny<CancellationToken>()))
            .ReturnsAsync((MatchService.Domain.Administrator?)null);

        Assert.True(await CreateService().UpdateModelAsync(new AdministratorModel(administrator.Id, "Updated")));
        Assert.True(await CreateService().DeleteModelAsync(administrator.Id));
        repository.Verify(x => x.UpdateAsync(administrator, It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(x => x.DeleteAsync(administrator, It.IsAny<CancellationToken>()), Times.Once);
    }

    private AdministratorApplicationService CreateService()
        => new(repository.Object, mapper);
}

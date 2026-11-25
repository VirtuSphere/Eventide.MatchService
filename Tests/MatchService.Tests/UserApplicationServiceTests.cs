using AutoMapper;
using MatchService.Application.Models.User;
using MatchService.Application.Services;
using MatchService.Application.Services.Mapping;
using MatchService.Domain.Repositories.Abstractions.Repositories;
using MatchService.ValueObjects;
using Moq;
using Xunit;

namespace MatchService.Tests;

public sealed class UserApplicationServiceTests
{
    private readonly Mock<IUserRepository> repository = new();
    private readonly IMapper mapper = new MapperConfiguration(
        configuration => configuration.AddProfile<ApplicationProfile>()).CreateMapper();

    [Fact]
    public async Task GetModelsAndGetByIdMapUsersAndHandleMissing()
    {
        var user = TestData.User();
        repository.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new[] { user });
        repository.Setup(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        repository.Setup(x => x.GetByIdAsync(It.Is<Guid>(id => id != user.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync((MatchService.Domain.User?)null);
        var service = CreateService();

        Assert.Equal(user.Username.Value, Assert.Single(await service.GetModelsAsync()).Username);
        Assert.Equal(user.Id, (await service.GetModelByIdAsync(user.Id))!.Id);
        Assert.Null(await service.GetModelByIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetUserByUsernameMapsUserAndHandlesMissing()
    {
        var user = TestData.User();
        repository.Setup(x => x.GetUserByUsernameAsync("PlayerOne", It.IsAny<CancellationToken>())).ReturnsAsync(user);
        repository.Setup(x => x.GetUserByUsernameAsync("Missing", It.IsAny<CancellationToken>()))
            .ReturnsAsync((MatchService.Domain.User?)null);
        var service = CreateService();

        Assert.Equal(user.Id, (await service.GetUserByUsernameAsync("PlayerOne"))!.Id);
        Assert.Null(await service.GetUserByUsernameAsync("Missing"));
    }

    [Fact]
    public async Task CreateRejectsDuplicateAndPersistsNewUser()
    {
        repository.Setup(x => x.GetUserByUsernameAsync("Existing", It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestData.User("Existing"));
        repository.Setup(x => x.GetUserByUsernameAsync("NewPlayer", It.IsAny<CancellationToken>()))
            .ReturnsAsync((MatchService.Domain.User?)null);
        var service = CreateService();

        Assert.Null(await service.CreateModelAsync(new CreateUserModel("Existing")));
        var created = await service.CreateModelAsync(new CreateUserModel("NewPlayer"));
        Assert.NotNull(created);
        Assert.Equal("NewPlayer", created.Username);
        repository.Verify(x => x.AddAsync(It.IsAny<MatchService.Domain.User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateHandlesMissingDuplicateAndExistingUser()
    {
        var user = TestData.User();
        repository.Setup(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        repository.Setup(x => x.GetByIdAsync(It.Is<Guid>(id => id != user.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync((MatchService.Domain.User?)null);
        repository.Setup(x => x.GetUserByUsernameAsync("Taken", It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestData.User("Taken"));
        repository.Setup(x => x.GetUserByUsernameAsync("Updated", It.IsAny<CancellationToken>()))
            .ReturnsAsync((MatchService.Domain.User?)null);
        repository.Setup(x => x.GetUserByUsernameAsync(user.Username.Value, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        var service = CreateService();

        Assert.False(await service.UpdateModelAsync(new UserModel(Guid.NewGuid(), "Anything")));
        Assert.False(await service.UpdateModelAsync(new UserModel(user.Id, "Taken")));
        Assert.True(await service.UpdateModelAsync(new UserModel(user.Id, "Updated")));
        Assert.True(await service.UpdateModelAsync(new UserModel(user.Id, user.Username.Value)));
        repository.Verify(x => x.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteHandlesMissingAndExistingUser()
    {
        var user = TestData.User();
        repository.Setup(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        repository.Setup(x => x.GetByIdAsync(It.Is<Guid>(id => id != user.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync((MatchService.Domain.User?)null);
        var service = CreateService();

        Assert.False(await service.DeleteModelAsync(Guid.NewGuid()));
        Assert.True(await service.DeleteModelAsync(user.Id));
        repository.Verify(x => x.DeleteAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }

    private UserApplicationService CreateService() => new(repository.Object, mapper);
}

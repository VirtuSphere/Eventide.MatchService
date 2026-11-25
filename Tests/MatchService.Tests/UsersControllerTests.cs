using MatchService.Application.Models.User;
using MatchService.Application.Services.Abstractions;
using MatchService.WebHost.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace MatchService.Tests;

public sealed class UsersControllerTests
{
    private readonly Mock<IUserApplicationService> service = new();

    [Fact]
    public async Task GetAllReturnsUsersAndGetByIdHandlesFoundAndMissing()
    {
        var user = new UserModel(Guid.NewGuid(), "Player");
        service.Setup(x => x.GetModelsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new[] { user });
        service.Setup(x => x.GetModelByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        service.Setup(x => x.GetModelByIdAsync(It.Is<Guid>(id => id != user.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserModel?)null);
        var controller = CreateController();

        Assert.IsType<OkObjectResult>((await controller.GetAll(CancellationToken.None)).Result);
        Assert.IsType<OkObjectResult>((await controller.GetById(user.Id, CancellationToken.None)).Result);
        Assert.IsType<NotFoundResult>((await controller.GetById(Guid.NewGuid(), CancellationToken.None)).Result);
    }

    [Fact]
    public async Task GetByUsernameHandlesFoundAndMissing()
    {
        var user = new UserModel(Guid.NewGuid(), "Player");
        service.Setup(x => x.GetUserByUsernameAsync("Player", It.IsAny<CancellationToken>())).ReturnsAsync(user);
        service.Setup(x => x.GetUserByUsernameAsync("Missing", It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserModel?)null);
        var controller = CreateController();

        Assert.IsType<OkObjectResult>((await controller.GetByUsername("Player", CancellationToken.None)).Result);
        Assert.IsType<NotFoundResult>((await controller.GetByUsername("Missing", CancellationToken.None)).Result);
    }

    [Fact]
    public async Task CreateReturnsCreatedOrConflict()
    {
        var user = new UserModel(Guid.NewGuid(), "Player");
        service.Setup(x => x.CreateModelAsync(It.IsAny<CreateUserModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        var controller = CreateController();

        var created = Assert.IsType<CreatedAtActionResult>(
            (await controller.Create(new CreateUserModel("Player"), CancellationToken.None)).Result);
        Assert.Equal(nameof(UsersController.GetById), created.ActionName);

        service.Setup(x => x.CreateModelAsync(It.IsAny<CreateUserModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserModel?)null);
        Assert.IsType<ConflictResult>(
            (await controller.Create(new CreateUserModel("Player"), CancellationToken.None)).Result);
    }

    [Fact]
    public async Task UpdateReturnsNoContentNotFoundOrConflict()
    {
        var id = Guid.NewGuid();
        service.Setup(x => x.UpdateModelAsync(It.IsAny<UserModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        var controller = CreateController();
        Assert.IsType<NoContentResult>(
            await controller.Update(id, new CreateUserModel("Updated"), CancellationToken.None));

        service.Setup(x => x.UpdateModelAsync(It.IsAny<UserModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        service.Setup(x => x.GetModelByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserModel?)null);
        Assert.IsType<NotFoundResult>(
            await controller.Update(id, new CreateUserModel("Updated"), CancellationToken.None));

        service.Setup(x => x.GetModelByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserModel(id, "Existing"));
        Assert.IsType<ConflictResult>(
            await controller.Update(id, new CreateUserModel("Existing"), CancellationToken.None));
    }

    [Fact]
    public async Task DeleteReturnsNoContentOrNotFound()
    {
        service.Setup(x => x.DeleteModelAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var controller = CreateController();
        Assert.IsType<NoContentResult>(await controller.Delete(Guid.NewGuid(), CancellationToken.None));

        service.Setup(x => x.DeleteModelAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        Assert.IsType<NotFoundResult>(await controller.Delete(Guid.NewGuid(), CancellationToken.None));
    }

    private UsersController CreateController() => new(service.Object);
}

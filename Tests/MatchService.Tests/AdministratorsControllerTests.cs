using MatchService.Application.Models.Administrator;
using MatchService.Application.Services.Abstractions;
using MatchService.WebHost.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace MatchService.Tests;

public sealed class AdministratorsControllerTests
{
    private readonly Mock<IAdministratorApplicationService> service = new();

    [Fact]
    public async Task GetById_ReturnsNotFoundWhenMissing()
    {
        service.Setup(x => x.GetModelByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AdministratorModel?)null);

        var result = await CreateController().GetById(Guid.NewGuid(), CancellationToken.None);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction()
    {
        var model = new AdministratorModel(Guid.NewGuid(), "AdminUser");
        service.Setup(x => x.CreateModelAsync(It.IsAny<CreateAdministratorModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(model);

        var result = await CreateController().Create(new CreateAdministratorModel("AdminUser"), CancellationToken.None);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(AdministratorsController.GetById), created.ActionName);
    }

    [Fact]
    public async Task Create_ReturnsConflictForDuplicate()
    {
        service.Setup(x => x.CreateModelAsync(It.IsAny<CreateAdministratorModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AdministratorModel?)null);

        var result = await CreateController().Create(new CreateAdministratorModel("AdminUser"), CancellationToken.None);

        Assert.IsType<ConflictResult>(result.Result);
    }

    [Fact]
    public async Task Delete_ReturnsExpectedStatus()
    {
        service.Setup(x => x.DeleteModelAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        Assert.IsType<NotFoundResult>(await CreateController().Delete(Guid.NewGuid(), CancellationToken.None));

        service.Setup(x => x.DeleteModelAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        Assert.IsType<NoContentResult>(await CreateController().Delete(Guid.NewGuid(), CancellationToken.None));
    }

    [Fact]
    public async Task GetAllAndGetByUsername_ReturnResults()
    {
        var model = new AdministratorModel(Guid.NewGuid(), "AdminUser");
        service.Setup(x => x.GetModelsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { model });
        service.Setup(x => x.GetAdministratorByUsernameAsync("AdminUser", It.IsAny<CancellationToken>()))
            .ReturnsAsync(model);

        var all = await CreateController().GetAll(CancellationToken.None);
        var byUsername = await CreateController().GetByUsername("AdminUser", CancellationToken.None);

        Assert.IsType<OkObjectResult>(all.Result);
        Assert.IsType<OkObjectResult>(byUsername.Result);
    }

    [Fact]
    public async Task Update_ReturnsNotFoundConflictOrNoContent()
    {
        service.Setup(x => x.UpdateModelAsync(It.IsAny<AdministratorModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        service.Setup(x => x.GetModelByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AdministratorModel?)null);
        Assert.IsType<NotFoundResult>(await CreateController().Update(
            Guid.NewGuid(), new CreateAdministratorModel("AdminUser"), CancellationToken.None));

        service.Setup(x => x.GetModelByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AdministratorModel(Guid.NewGuid(), "Existing"));
        Assert.IsType<ConflictResult>(await CreateController().Update(
            Guid.NewGuid(), new CreateAdministratorModel("Existing"), CancellationToken.None));

        service.Setup(x => x.UpdateModelAsync(It.IsAny<AdministratorModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        Assert.IsType<NoContentResult>(await CreateController().Update(
            Guid.NewGuid(), new CreateAdministratorModel("Updated"), CancellationToken.None));
    }

    private AdministratorsController CreateController() => new(service.Object);
}

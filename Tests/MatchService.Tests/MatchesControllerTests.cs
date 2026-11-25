using MatchService.Application.Models.Match;
using MatchService.Application.Services.Abstractions;
using MatchService.WebHost.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace MatchService.Tests;

public sealed class MatchesControllerTests
{
    private readonly Mock<IMatchApplicationService> service = new();

    [Fact]
    public async Task GetAllReturnsMatchesAndGetByIdHandlesFoundAndMissing()
    {
        var match = CreateModel();
        service.Setup(x => x.GetModelsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new[] { match });
        service.Setup(x => x.GetModelByIdAsync(match.Id, It.IsAny<CancellationToken>())).ReturnsAsync(match);
        service.Setup(x => x.GetModelByIdAsync(It.Is<Guid>(id => id != match.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync((MatchModel?)null);
        var controller = CreateController();

        Assert.IsType<OkObjectResult>((await controller.GetAll(CancellationToken.None)).Result);
        Assert.IsType<OkObjectResult>((await controller.GetById(match.Id, CancellationToken.None)).Result);
        Assert.IsType<NotFoundResult>((await controller.GetById(Guid.NewGuid(), CancellationToken.None)).Result);
    }

    [Fact]
    public async Task CreateReturnsCreatedOrBadRequest()
    {
        var match = CreateModel();
        service.Setup(x => x.CreateModelAsync(It.IsAny<CreateMatchModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(match);
        var controller = CreateController();

        var created = Assert.IsType<CreatedAtActionResult>(
            (await controller.Create(new CreateMatchModel(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
                Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None)).Result);
        Assert.Equal(nameof(MatchesController.GetById), created.ActionName);

        service.Setup(x => x.CreateModelAsync(It.IsAny<CreateMatchModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((MatchModel?)null);
        Assert.IsType<BadRequestResult>(
            (await controller.Create(new CreateMatchModel(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
                Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None)).Result);
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

    private MatchesController CreateController() => new(service.Object);

    private static MatchModel CreateModel()
        => new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            null, MatchService.Domain.Enums.MatchStatus.Scheduled, null, null, null, null, null, null, null,
            DateTime.UtcNow);
}

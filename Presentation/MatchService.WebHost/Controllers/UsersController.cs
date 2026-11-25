using MatchService.Application.Models.User;
using MatchService.Application.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace MatchService.WebHost.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController(IUserApplicationService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<UserModel>>> GetAll(CancellationToken cancellationToken)
        => Ok(await service.GetModelsAsync(cancellationToken));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserModel>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var user = await service.GetModelByIdAsync(id, cancellationToken);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpGet("by-username/{username}")]
    [ProducesResponseType(typeof(UserModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserModel>> GetByUsername(string username, CancellationToken cancellationToken)
    {
        var user = await service.GetUserByUsernameAsync(username, cancellationToken);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPost]
    [ProducesResponseType(typeof(UserModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserModel>> Create(CreateUserModel model, CancellationToken cancellationToken)
    {
        var user = await service.CreateModelAsync(model, cancellationToken);
        if (user is null)
            return Conflict();

        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, CreateUserModel model, CancellationToken cancellationToken)
    {
        var updated = await service.UpdateModelAsync(new UserModel(id, model.Username), cancellationToken);
        if (updated)
            return NoContent();

        var user = await service.GetModelByIdAsync(id, cancellationToken);
        return user is null ? NotFound() : Conflict();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        => await service.DeleteModelAsync(id, cancellationToken) ? NoContent() : NotFound();
}

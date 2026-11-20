using MatchService.Application.Models.Administrator;
using MatchService.Application.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace MatchService.WebHost.Controllers;

[ApiController]
[Route("api/administrators")]
public sealed class AdministratorsController(IAdministratorApplicationService service) : ControllerBase
{
    /// <summary>
    /// Returns all administrators.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AdministratorModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AdministratorModel>>> GetAll(
        CancellationToken cancellationToken)
        => Ok(await service.GetModelsAsync(cancellationToken));

    /// <summary>
    /// Returns an administrator by identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AdministratorModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AdministratorModel>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var administrator = await service.GetModelByIdAsync(id, cancellationToken);
        return administrator is null ? NotFound() : Ok(administrator);
    }

    /// <summary>
    /// Returns an administrator by username.
    /// </summary>
    [HttpGet("by-username/{username}")]
    [ProducesResponseType(typeof(AdministratorModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AdministratorModel>> GetByUsername(
        string username,
        CancellationToken cancellationToken)
    {
        var administrator = await service.GetAdministratorByUsernameAsync(username, cancellationToken);
        return administrator is null ? NotFound() : Ok(administrator);
    }

    /// <summary>
    /// Creates an administrator.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(AdministratorModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AdministratorModel>> Create(
        CreateAdministratorModel model,
        CancellationToken cancellationToken)
    {
        var administrator = await service.CreateModelAsync(model, cancellationToken);
        if (administrator is null)
            return Conflict();

        return CreatedAtAction(nameof(GetById), new { id = administrator.Id }, administrator);
    }

    /// <summary>
    /// Updates an administrator username.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(
        Guid id,
        CreateAdministratorModel model,
        CancellationToken cancellationToken)
    {
        var updated = await service.UpdateModelAsync(
            new AdministratorModel(id, model.Username),
            cancellationToken);
        if (updated)
            return NoContent();

        var administrator = await service.GetModelByIdAsync(id, cancellationToken);
        return administrator is null ? NotFound() : Conflict();
    }

    /// <summary>
    /// Deletes an administrator.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        => await service.DeleteModelAsync(id, cancellationToken) ? NoContent() : NotFound();
}

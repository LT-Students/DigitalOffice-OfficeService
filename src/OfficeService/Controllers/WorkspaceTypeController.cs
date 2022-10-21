using System;
using System.Threading;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.OfficeService.Business.Commands.WorkspaceType.Create;
using LT.DigitalOffice.OfficeService.Business.Commands.WorkspaceType.Edit;
using LT.DigitalOffice.OfficeService.Business.Commands.WorkspaceType.Find;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.OfficeService.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class WorkspaceTypeController : ControllerBase
  {
    private readonly IMediator _mediator;
    private readonly IAccessValidator _accessValidator;

    public WorkspaceTypeController(
      IMediator mediator,
      IAccessValidator accessValidator)
    {
      _mediator = mediator;
      _accessValidator = accessValidator;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateAsync(
      [FromBody] CreateWorkspaceTypeRequest request,
      CancellationToken ct)
    {
      if (!await _accessValidator.HasRightsAsync(
           Rights.AddEditRemoveCompanyData,
           Rights.AddEditRemoveCompanies))
      {
        return StatusCode(403);
      }

      Guid? result = await _mediator.Send(request, ct);
      return result is null
        ? BadRequest()
        : Created("/workspaceTypes", result);
    }

    [HttpGet("find")]
    public async Task<IActionResult> FindAsync(
      [FromQuery] WorkspaceTypeFindFilter filter,
      CancellationToken ct)
    {
      return Ok(await _mediator.Send(filter, ct));
    }

    [HttpPatch("edit")]
    public async Task<IActionResult> EditAsync(
      [FromQuery] Guid workspaceTypeId,
      [FromBody] JsonPatchDocument<EditWorkspaceTypePatch> patch,
      CancellationToken ct)
    {
      if (!await _accessValidator.HasRightsAsync(Rights.AddEditRemoveCompanyData)
           && !await _accessValidator.HasRightsAsync(Rights.AddEditRemoveCompanies))
      {
        return StatusCode(403);
      }

      EditWorkspaceTypeRequest request = new()
      {
        WorkspaceTypeId = workspaceTypeId,
        Patch = patch
      };

      bool result = await _mediator.Send(request, ct);
      return result
        ? Ok(true)
        : BadRequest();
    }
  }
}

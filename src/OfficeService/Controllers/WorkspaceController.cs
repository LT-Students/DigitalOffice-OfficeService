using System;
using System.Threading;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.OfficeService.Business.Workspace.Create;
using LT.DigitalOffice.OfficeService.Business.Workspace.Find;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.OfficeService.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class WorkspaceController : ControllerBase
  {
    private readonly IMediator _mediator;
    private readonly IAccessValidator _accessValidator;

    public WorkspaceController(
      IMediator mediator,
      IAccessValidator accessValidator)
    {
      _mediator = mediator;
      _accessValidator = accessValidator;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateAsync(
      [FromBody] CreateWorkspaceRequest request,
      CancellationToken ct)
    {
      if (!await _accessValidator.HasRightsAsync(Rights.AddEditRemoveOffices))
      {
        return StatusCode(403);
      }

      Guid? result = await _mediator.Send(request, ct);
      return result is null
        ? BadRequest()
        : Created("/workspaces", result);
    }

    [HttpGet("find")]
    public async Task<IActionResult> FindAsync(
      [FromQuery] WorkspaceFindFilter filter,
      CancellationToken ct)
    {
      return Ok(await _mediator.Send(filter, ct));
    }
  }
}

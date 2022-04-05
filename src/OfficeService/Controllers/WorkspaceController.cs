using System;
using System.Threading.Tasks;

using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.OfficeService.Business.Commands.Workspace.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Dto.Models.Workspace;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Workspace;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Workspace.Filters;

using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.OfficeService.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class WorkspaceController : ControllerBase
  {
    [HttpPost("create")]
    public async Task<OperationResultResponse<Guid?>> CreateAsync(
      [FromServices] ICreateWorkspaceCommand  command,
      [FromBody] CreateWorkspaceRequest request)
    {
      return await command.ExecuteAsync(request);
    }

    [HttpGet("find")]
    public async Task<FindResultResponse<WorkspaceInfo>> FindAsync(
      [FromServices] IFindWorkspacesCommand command,
      [FromQuery] WorkspaceFindFilter filter)
    {
      return await command.ExecuteAsync(filter);
    }
  }
}

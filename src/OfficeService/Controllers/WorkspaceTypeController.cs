using System;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.OfficeService.Business.Commands.WorkspaceType.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Dto.Models.Workspace;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.WorkspaceType;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.WorkspaceType.Filters;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.OfficeService.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class WorkspaceTypeController : ControllerBase
  {
    [HttpPost("create")]
    public async Task<OperationResultResponse<Guid?>> CreateAsync(
      [FromServices] ICreateWorkspaceTypeCommand command,
      [FromBody] CreateWorkspaceTypeRequest request)
    {
      return await command.ExecuteAsync(request);
    }

    [HttpGet("find")]
    public async Task<FindResultResponse<WorkspaceTypeInfo>> FindAsync(
      [FromServices] IFindWorkspaceTypesCommand command,
      [FromQuery] WorkspaceTypeFindFilter filter)
    {
      return await command.ExecuteAsync(filter);
    }

    [HttpPatch("edit")]
    public async Task<OperationResultResponse<bool>> EditAsync(
      [FromServices] IEditWorkspaceTypeCommand command,
      [FromQuery] Guid workspaceTypeId,
      [FromBody] JsonPatchDocument<EditWorkspaceTypeRequest> request)
    {
      return await command.Execute(workspaceTypeId, request);
    }
  }
}

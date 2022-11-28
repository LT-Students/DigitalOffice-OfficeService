using DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Kernel.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.OfficeService.Business.Workspace.Find
{
  public record WorkspaceFindFilter : BaseFindFilter, IRequest<FindResult<WorkspaceInfo>>
  {
    [FromQuery(Name = "IncludeDeactivated")]
    public bool IncludeDeactivated { get; set; } = false;
  }
}

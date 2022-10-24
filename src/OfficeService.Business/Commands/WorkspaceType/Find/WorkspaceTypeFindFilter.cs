using DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Kernel.Requests;
using LT.DigitalOffice.OfficeService.Models.Dto.Models.Workspace;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.OfficeService.Business.Commands.WorkspaceType.Find
{
  public record WorkspaceTypeFindFilter : BaseFindFilter, IRequest<FindResult<WorkspaceTypeInfo>>
  {
    [FromQuery(Name = "IncludeDeactivated")]
    public bool IncludeDeactivated { get; set; } = false;
  }
}

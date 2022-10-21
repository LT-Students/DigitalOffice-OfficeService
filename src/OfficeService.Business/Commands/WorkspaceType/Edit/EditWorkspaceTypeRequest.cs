using System;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.OfficeService.Business.Commands.WorkspaceType.Edit
{
  public record EditWorkspaceTypeRequest : IRequest<bool>
  {
    public Guid WorkspaceTypeId { get; set; }
    public JsonPatchDocument<EditWorkspaceTypePatch> Patch { get; set; }
  }
}

using System;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.WorkspaceType;

namespace LT.DigitalOffice.OfficeService.Models.Dto.Requests.Workspace
{
  public record CreateWorkspaceRequest
  {
    public Guid? ParentId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public bool IsBookable { get; set; }

    public Guid? WorkspaceTypeId { get; set; }

    public CreateWorkspaceTypeRequest WorkspaceType { get; set; }
  }
}

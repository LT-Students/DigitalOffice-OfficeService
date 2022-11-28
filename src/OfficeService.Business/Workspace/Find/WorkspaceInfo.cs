using System;

namespace LT.DigitalOffice.OfficeService.Business.Workspace.Find
{
  public record WorkspaceInfo
  {
    public Guid Id { get; set; }

    public Guid? ParentId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public bool IsActive { get; set; }

    public WorkspaceTypeInfo WorkspaceType { get; set; }
  }
}

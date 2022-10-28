using System;
using LT.DigitalOffice.OfficeService.Business.WorkspaceType.Create;
using MediatR;

namespace LT.DigitalOffice.OfficeService.Business.Workspace.Create
{
  public record CreateWorkspaceRequest : IRequest<Guid?>
  {
    public Guid? ParentId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public bool IsBookable { get; set; }

    public Guid? WorkspaceTypeId { get; set; }

    public CreateWorkspaceTypeRequest WorkspaceType { get; set; }
  }
}

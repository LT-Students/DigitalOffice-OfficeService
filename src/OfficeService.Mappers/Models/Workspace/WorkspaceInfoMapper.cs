using LT.DigitalOffice.OfficeService.Mappers.Models.Workspace.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Db;
using LT.DigitalOffice.OfficeService.Models.Dto.Models.Workspace;

namespace LT.DigitalOffice.OfficeService.Mappers.Models.Workspace
{
  public class WorkspaceInfoMapper : IWorkspaceInfoMapper
  {
    public WorkspaceInfo Map(DbWorkspace workspace)
    {
      if (workspace == null)
      {
        return null;
      }

      return new WorkspaceInfo
      {
        Id = workspace.Id,
        ParentId = workspace.ParentId,
        WorkspaceTypeId = workspace.WorkspaceTypeId,
        Name = workspace.Name,
        Description = workspace.Description,
        IsActive = workspace.IsActive
      };
    }
  }
}

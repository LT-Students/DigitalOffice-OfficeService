using LT.DigitalOffice.OfficeService.Mappers.Models.Workspace.Interfaces;
using LT.DigitalOffice.OfficeService.Mappers.Models.WorkspaceType.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Db;
using LT.DigitalOffice.OfficeService.Models.Dto.Models.Workspace;

namespace LT.DigitalOffice.OfficeService.Mappers.Models.Workspace
{
  public class WorkspaceInfoMapper : IWorkspaceInfoMapper
  {
    private readonly IWorkspaceTypeInfoMapper _workspaceTypeInfoMapper;

    public WorkspaceInfoMapper(IWorkspaceTypeInfoMapper workspaceTypeInfoMapper)
    {
      _workspaceTypeInfoMapper = workspaceTypeInfoMapper;
    }

    public WorkspaceInfo Map(DbWorkspace workspace)
    {
      if (workspace is null)
      {
        return null;
      }

      return new WorkspaceInfo
      {
        Id = workspace.Id,
        ParentId = workspace.ParentId,
        Name = workspace.Name,
        Description = workspace.Description,
        IsActive = workspace.IsActive,
        WorkspaceType = _workspaceTypeInfoMapper.Map(workspace.WorkspaceType)
      };
    }
  }
}

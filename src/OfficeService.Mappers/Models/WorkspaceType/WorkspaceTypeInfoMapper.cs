using LT.DigitalOffice.OfficeService.Mappers.Models.WorkspaceType.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Db;
using LT.DigitalOffice.OfficeService.Models.Dto.Models.Workspace;

namespace LT.DigitalOffice.OfficeService.Mappers.Models.WorkspaceType
{
  public class WorkspaceTypeInfoMapper : IWorkspaceTypeInfoMapper
  {
    public WorkspaceTypeInfo Map(DbWorkspaceType workspaceType)
    {
      if (workspaceType == null)
      {
        return null;
      }

      return new WorkspaceTypeInfo
      {
        Name = workspaceType.Name,
        Description = workspaceType.Description,
        StartTime = workspaceType.StartTime,
        EndTime = workspaceType.EndTime,
        BookingRule = workspaceType.BookingRule,
        IsActive = workspaceType.IsActive
      };
    }
  }
}

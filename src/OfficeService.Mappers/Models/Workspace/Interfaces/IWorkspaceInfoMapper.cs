using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.OfficeService.Models.Db;
using LT.DigitalOffice.OfficeService.Models.Dto.Models.Workspace;

namespace LT.DigitalOffice.OfficeService.Mappers.Models.Workspace.Interfaces
{
  [AutoInject]
  public interface IWorkspaceInfoMapper
  {
    WorkspaceInfo Map(DbWorkspace workspace);
  }
}

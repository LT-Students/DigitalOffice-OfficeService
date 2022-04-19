using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.OfficeService.Models.Db;
using LT.DigitalOffice.OfficeService.Models.Dto.Models.Workspace;

namespace LT.DigitalOffice.OfficeService.Mappers.Models.WorkspaceType.Interfaces
{
  [AutoInject]
  public interface IWorkspaceTypeInfoMapper
  {
    WorkspaceTypeInfo Map(DbWorkspaceType workspaceType);
  }
}

using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.OfficeService.Models.Db;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Workspace;

namespace LT.DigitalOffice.OfficeService.Mappers.Db.Workspace.Interfaces
{
  [AutoInject]
  public interface IDbWorkspaceMapper
  {
    DbWorkspace Map(CreateWorkspaceRequest request);
  }
}

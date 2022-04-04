using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.OfficeService.Models.Db;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.WorkspaceType;

namespace LT.DigitalOffice.OfficeService.Mappers.Db.WorkspaceType.Interfaces
{
  [AutoInject]
  public interface IDbWorkspaceTypeMapper
  {
    DbWorkspaceType Map(CreateWorkspaceTypeRequest request);
  }
}

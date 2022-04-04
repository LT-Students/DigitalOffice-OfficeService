using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.OfficeService.Models.Db;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.WorkspaceType.Filters;

namespace LT.DigitalOffice.OfficeService.Data.WorkspaceType.Interfaces
{
  [AutoInject]
  public interface IWorkspaceTypeRepository
  {
    Task CreateAsync(DbWorkspaceType workspaceType);

    Task<DbWorkspaceType> GetAsync(Guid workspaceTypeId);

    Task<List<DbWorkspaceType>> GetAsync(List<Guid> workspaceTypeIds);

    Task<(List<DbWorkspaceType>, int totalCount)> FindAsync(WorkspaceTypeFindFilter filter);
  }
}

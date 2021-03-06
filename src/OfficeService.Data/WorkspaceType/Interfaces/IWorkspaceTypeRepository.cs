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
    Task<Guid?> CreateAsync(DbWorkspaceType workspaceType);

    Task<(List<DbWorkspaceType>, int totalCount)> FindAsync(WorkspaceTypeFindFilter filter);

    Task<bool> DoesNameExistAsync(string name);
  }
}

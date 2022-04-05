using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.OfficeService.Models.Db;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Workspace.Filters;

namespace LT.DigitalOffice.OfficeService.Data.Workspace.Interfaces
{
  [AutoInject]
  public interface IWorkspaceRepository
  {
    Task<Guid?> CreateAsync(DbWorkspace workspace);

    Task<DbWorkspace> GetAsync(Guid workspaceId);

    Task<List<DbWorkspace>> GetAsync(List<Guid> workspaceIds);

    Task<(List<DbWorkspace>, int totalCount)> FindAsync(WorkspaceFindFilter filter);
  }
}

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

    Task<(List<DbWorkspace>, int totalCount)> FindAsync(WorkspaceFindFilter filter);
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.OfficeService.Data.Provider;
using LT.DigitalOffice.OfficeService.Data.Workspace.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Db;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Workspace.Filters;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.OfficeService.Data.Workspace
{
  public class WorkspaceRepository : IWorkspaceRepository
  {
    private readonly IDataProvider _provider;

    public WorkspaceRepository(IDataProvider provider)
    {
      _provider = provider;
    }

    public async Task<Guid?> CreateAsync(DbWorkspace workspace)
    {
      if (workspace is null)
      {
        return null;
      }

      _provider.Workspaces.Add(workspace);
      await _provider.SaveAsync();

      return workspace.Id;
    }

    public async Task<DbWorkspace> GetAsync(Guid workspaceId)
    {
      return await _provider.Workspaces
        .Include(w => w.WorkspaceType)
        .FirstOrDefaultAsync(x =>
          x.Id == workspaceId
          && x.IsActive
          && x.WorkspaceType.IsActive);
    }

    public async Task<(List<DbWorkspace>, int totalCount)> FindAsync(WorkspaceFindFilter filter)
    {
      if (filter is null)
      {
        return (null, 0);
      }

      IQueryable<DbWorkspace> dbWorkspaces = _provider.Workspaces
        .AsQueryable();

      if (!filter.IncludeDeactivated)
      {
        dbWorkspaces = dbWorkspaces.Where(x => x.IsActive);
      }

      return (
        await dbWorkspaces
          .Include(w => w.WorkspaceType)
          .Skip(filter.SkipCount)
          .Take(filter.TakeCount)
          .ToListAsync(),
        await dbWorkspaces.CountAsync());
    }
  }
}

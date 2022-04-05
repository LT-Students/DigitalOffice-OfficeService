using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using LT.DigitalOffice.OfficeService.Data.Provider;
using LT.DigitalOffice.OfficeService.Data.Workspace.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Db;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Workspace.Filters;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.OfficeService.Data.Workspace
{
  public class WorkspaceRepository : IWorkspaceRepository
  {
    private readonly IDataProvider _provider;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<WorkspaceRepository> _logger;

    public WorkspaceRepository(
      IDataProvider provider,
      IHttpContextAccessor httpContextAccessor,
      ILogger<WorkspaceRepository> logger)
    {
      _provider = provider;
      _httpContextAccessor = httpContextAccessor;
      _logger = logger;
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
      return await _provider.Workspaces.FirstOrDefaultAsync(x => x.Id == workspaceId);
    }

    public async Task<List<DbWorkspace>> GetAsync(List<Guid> workspaceIds)
    {
      return await _provider.Workspaces
        .Where(o => workspaceIds.Contains(o.Id))
        .ToListAsync();
    }

    public async Task<(List<DbWorkspace>, int totalCount)> FindAsync(WorkspaceFindFilter filter)
    {
      if (filter == null)
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
        await dbWorkspaces.Skip(filter.SkipCount).Take(filter.TakeCount).ToListAsync(), await dbWorkspaces.CountAsync());
    }
  }
}

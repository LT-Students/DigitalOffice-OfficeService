using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using LT.DigitalOffice.OfficeService.Data.Provider;
using LT.DigitalOffice.OfficeService.Data.Workspace;
using LT.DigitalOffice.OfficeService.Data.WorkspaceType.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Db;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.WorkspaceType.Filters;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.OfficeService.Data.WorkspaceType
{
  public class WorkspaceTypeRepository : IWorkspaceTypeRepository
  {
    private readonly IDataProvider _provider;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<WorkspaceRepository> _logger;

    public WorkspaceTypeRepository(
      IDataProvider provider,
      IHttpContextAccessor httpContextAccessor,
      ILogger<WorkspaceRepository> logger)
    {
      _provider = provider;
      _httpContextAccessor = httpContextAccessor;
      _logger = logger;
    }

    public async Task<Guid?> CreateAsync(DbWorkspaceType workspaceType)
    {
      if (workspaceType is null)
      {
        return null;
      }

      _provider.WorkspacesTypes.Add(workspaceType);
      await _provider.SaveAsync();

      return workspaceType.Id;
    }

    public async Task<(List<DbWorkspaceType>, int totalCount)> FindAsync(WorkspaceTypeFindFilter filter)
    {
      if (filter is null)
      {
        return (null, 0);
      }

      IQueryable<DbWorkspaceType> dbWorkspaceTypes = _provider.WorkspacesTypes
        .AsQueryable();

      if (!filter.IncludeDeactivated)
      {
        dbWorkspaceTypes = dbWorkspaceTypes.Where(x => x.IsActive);
      }

      return (
        await dbWorkspaceTypes.Skip(filter.SkipCount).Take(filter.TakeCount).ToListAsync(), await dbWorkspaceTypes.CountAsync());
    }
  }
}

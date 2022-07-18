using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.OfficeService.Data.Provider;
using LT.DigitalOffice.OfficeService.Data.WorkspaceType.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Db;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.WorkspaceType.Filters;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.OfficeService.Data.WorkspaceType
{
  public class WorkspaceTypeRepository : IWorkspaceTypeRepository
  {
    private readonly IDataProvider _provider;

    public WorkspaceTypeRepository(
      IDataProvider provider)
    {
      _provider = provider;
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

    public async Task<bool> IsNameTaken(string name)
    {
      return await _provider.WorkspacesTypes.AnyAsync(wt => string.Equals(wt.Name, name));
    }
  }
}

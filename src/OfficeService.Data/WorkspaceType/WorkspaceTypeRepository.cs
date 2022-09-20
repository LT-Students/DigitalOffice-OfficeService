using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.OfficeService.Data.Provider;
using LT.DigitalOffice.OfficeService.Data.WorkspaceType.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Db;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.WorkspaceType.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.OfficeService.Data.WorkspaceType
{
  public class WorkspaceTypeRepository : IWorkspaceTypeRepository
  {
    private readonly IDataProvider _provider;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public WorkspaceTypeRepository(
      IDataProvider provider,
      IHttpContextAccessor httpContextAccessor)
    {
      _provider = provider;
      _httpContextAccessor = httpContextAccessor;
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

    public async Task<bool> DoesNameExistAsync(string name)
    {
      return await _provider.WorkspacesTypes.AnyAsync(wt => string.Equals(wt.Name, name));
    }

    public async Task<bool> EditAsync(Guid workspaceId, JsonPatchDocument<DbWorkspaceType> request)
    {
      DbWorkspaceType dbWorkspaceType = await _provider.WorkspacesTypes
        .FirstOrDefaultAsync(x => x.Id == workspaceId);  

      if (dbWorkspaceType is null || request is null)
      {
        return false;
      }

      request.ApplyTo(dbWorkspaceType);
      dbWorkspaceType.ModifiedBy = _httpContextAccessor.HttpContext.GetUserId();
      dbWorkspaceType.ModifiedAtUtc = DateTime.UtcNow;
      await _provider.SaveAsync();

      return true;
    }
  }
}

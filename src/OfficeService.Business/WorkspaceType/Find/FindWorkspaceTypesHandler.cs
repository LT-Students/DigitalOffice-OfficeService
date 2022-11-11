using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.OfficeService.Broker.Requests;
using LT.DigitalOffice.OfficeService.Business.Workspace.Find;
using LT.DigitalOffice.OfficeService.DataLayer;
using LT.DigitalOffice.OfficeService.DataLayer.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.OfficeService.Business.WorkspaceType.Find
{
  public class FindWorkspaceTypesHandler : IRequestHandler<WorkspaceTypeFindFilter, FindResult<WorkspaceTypeInfo>>
  {
    private readonly OfficeServiceDbContext _dbContext;

    #region private methods

    private async Task<(List<DbWorkspaceType>, int totalCount)> FindWorkspaceTypesAsync(WorkspaceTypeFindFilter filter)
    {
      if (filter is null)
      {
        return (null, 0);
      }

      IQueryable<DbWorkspaceType> dbWorkspaceTypes = _dbContext.WorkspacesTypes
        .AsNoTracking()
        .AsQueryable();

      if (!filter.IncludeDeactivated)
      {
        dbWorkspaceTypes = dbWorkspaceTypes.Where(x => x.IsActive);
      }

      return (await dbWorkspaceTypes
          .Skip(filter.SkipCount)
          .Take(filter.TakeCount)
          .ToListAsync(),
        await dbWorkspaceTypes.CountAsync());
    }

    public WorkspaceTypeInfo Map(DbWorkspaceType workspaceType)
    {
      if (workspaceType is null)
      {
        return null;
      }

      return new WorkspaceTypeInfo
      {
        Id = workspaceType.Id,
        Name = workspaceType.Name,
        Description = workspaceType.Description,
        StartTime = workspaceType.StartTime,
        EndTime = workspaceType.EndTime,
        BookingRule = (BookingRule)workspaceType.BookingRule,
        IsActive = workspaceType.IsActive
      };
    }

    #endregion

    public FindWorkspaceTypesHandler(OfficeServiceDbContext dbContext)
    {
      _dbContext = dbContext;
    }

    public async Task<FindResult<WorkspaceTypeInfo>> Handle(WorkspaceTypeFindFilter filter, CancellationToken ct)
    {
      (List<DbWorkspaceType> workspaceTypes, int totalCount) = await FindWorkspaceTypesAsync(filter);

      return new FindResult<WorkspaceTypeInfo>
      {
        Body = workspaceTypes.ConvertAll(Map),
        TotalCount = totalCount
      };
    }
  }
}

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Validators.Interfaces;
using LT.DigitalOffice.OfficeService.Broker.Requests;
using LT.DigitalOffice.OfficeService.Data.Provider;
using LT.DigitalOffice.OfficeService.DataLayer.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.OfficeService.Business.Workspace.Find
{
  public class FindWorkspacesHandler : IRequestHandler<WorkspaceFindFilter, FindResult<WorkspaceInfo>>
  {
    private readonly IBaseFindFilterValidator _baseFindValidator;
    private readonly IDataProvider _provider;

    #region private methods

    private async Task<(List<DbWorkspace>, int totalCount)> FindWorkspacesAsync(
      WorkspaceFindFilter filter,
      CancellationToken ct)
    {
      if (filter is null)
      {
        return (null, 0);
      }

      IQueryable<DbWorkspace> dbWorkspaces = _provider.Workspaces;

      if (!filter.IncludeDeactivated)
      {
        dbWorkspaces = dbWorkspaces.Where(x => x.IsActive);
      }

      return (
        await dbWorkspaces
          .Include(w => w.WorkspaceType)
          .Skip(filter.SkipCount)
          .Take(filter.TakeCount)
          .ToListAsync(ct),
        await dbWorkspaces.CountAsync(ct));
    }

    public WorkspaceInfo Map(DbWorkspace workspace)
    {
      if (workspace is null)
      {
        return null;
      }

      return new WorkspaceInfo
      {
        Id = workspace.Id,
        ParentId = workspace.ParentId,
        Name = workspace.Name,
        Description = workspace.Description,
        IsActive = workspace.IsActive,
        WorkspaceType = new WorkspaceTypeInfo
        {
          Id = workspace.WorkspaceType.Id,
          Name = workspace.WorkspaceType.Name,
          Description = workspace.WorkspaceType.Description,
          StartTime = workspace.WorkspaceType.StartTime,
          EndTime = workspace.WorkspaceType.EndTime,
          BookingRule = (BookingRule)workspace.WorkspaceType.BookingRule,
          IsActive = workspace.WorkspaceType.IsActive
        }
      };
    }

    #endregion

    public FindWorkspacesHandler(
      IBaseFindFilterValidator baseFindValidator,
      IDataProvider provider)
    {
      _baseFindValidator = baseFindValidator;
      _provider = provider;
    }

    public async Task<FindResult<WorkspaceInfo>> Handle(WorkspaceFindFilter filter, CancellationToken ct)
    {
      if (!_baseFindValidator.ValidateCustom(filter, out var errors))
      {
        throw new BadRequestException(errors);
      }

      (List<DbWorkspace> workspaces, int totalCount) = await FindWorkspacesAsync(filter, ct);

      return new FindResult<WorkspaceInfo> { Body = workspaces.ConvertAll(Map), TotalCount = totalCount };
    }
  }
}

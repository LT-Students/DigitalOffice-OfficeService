using System;
using System.Threading.Tasks;
using LT.DigitalOffice.OfficeService.Data.Provider;
using LT.DigitalOffice.OfficeService.Data.Workspace.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Db;
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

    public async Task<DbWorkspace> GetAsync(Guid workspaceId)
    {
      return await _provider.Workspaces
        .Include(w => w.WorkspaceType)
        .FirstOrDefaultAsync(x =>
          x.Id == workspaceId
          && x.IsActive
          && x.WorkspaceType.IsActive);
    }
  }
}

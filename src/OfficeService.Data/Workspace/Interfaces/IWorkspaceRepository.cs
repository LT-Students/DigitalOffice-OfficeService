using System;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.OfficeService.Models.Db;

namespace LT.DigitalOffice.OfficeService.Data.Workspace.Interfaces
{
  [AutoInject]
  public interface IWorkspaceRepository
  {
    Task<DbWorkspace> GetAsync(Guid workspaceId);
  }
}

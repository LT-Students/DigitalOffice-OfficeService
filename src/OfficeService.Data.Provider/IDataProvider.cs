using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.EFSupport.Provider;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.OfficeService.DataLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.OfficeService.Data.Provider
{
  [AutoInject(InjectType.Scoped)]
  public interface IDataProvider : IBaseDataProvider
  {
    DbSet<DbOffice> Offices { get; set; }
    DbSet<DbOfficeUser> OfficesUsers { get; set; }
    DbSet<DbWorkspace> Workspaces { get; set; }
    DbSet<DbWorkspaceType> WorkspacesTypes { get; set; }
    DbSet<DbWorkspaceImage> WorkspacesImages { get; set; }
    DbSet<DbWorkspaceTag> WorkspacesTags { get; set; }
  }
}

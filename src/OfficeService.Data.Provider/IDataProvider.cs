using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Database;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.OfficeService.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.OfficeService.Data.Provider
{
  [AutoInject(InjectType.Scoped)]
  public interface IDataProvider : IBaseDataProvider
  {
    DbSet<DbOffice> Offices { get; set; }
    DbSet<DbOfficeUser> OfficesUsers { get; set; }

    DbSet<DbWorkspace> Workspaces { get; set; }
    DbSet<DbWorkspaceType> WorkspaceTypes { get; set; }
  }
}

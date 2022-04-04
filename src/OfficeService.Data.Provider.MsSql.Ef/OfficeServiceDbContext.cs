using System.Reflection;
using System.Threading.Tasks;
using LT.DigitalOffice.OfficeService.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.OfficeService.Data.Provider.MsSql.Ef
{
  /// <summary>
  /// A class that defines the tables and its properties in the database of OfficeService.
  /// </summary>
  public class OfficeServiceDbContext : DbContext, IDataProvider
  {
    public OfficeServiceDbContext(DbContextOptions<OfficeServiceDbContext> options)
      : base(options)
    {
    }

    public DbSet<DbOffice> Offices { get; set; }
    public DbSet<DbOfficeUser> OfficesUsers { get; set; }
    public DbSet<DbWorkspace> Workspaces { get; set; }
    public DbSet<DbWorkspaceType> WorkspaceTypes { get; set; }

    // Fluent API is written here.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.ApplyConfigurationsFromAssembly(Assembly.Load("LT.DigitalOffice.OfficeService.Models.Db"));
    }

    public object MakeEntityDetached(object obj)
    {
      Entry(obj).State = EntityState.Detached;

      return Entry(obj).State;
    }

    public void Save()
    {
      SaveChanges();
    }

    public void EnsureDeleted()
    {
      Database.EnsureDeleted();
    }

    public bool IsInMemory()
    {
      return Database.IsInMemory();
    }

    public async Task SaveAsync()
    {
      await SaveChangesAsync();
    }
  }
}

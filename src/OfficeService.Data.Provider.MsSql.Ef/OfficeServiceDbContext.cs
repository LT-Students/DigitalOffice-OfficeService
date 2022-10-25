using System.Reflection;
using LT.DigitalOffice.OfficeService.DataLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.OfficeService.DataLayer
{
  /// <summary>
  /// A class that defines the tables and its properties in the database of OfficeService.
  /// </summary>
  public class OfficeServiceDbContext : DbContext
  {
    public DbSet<DbOffice> Offices { get; set; }
    public DbSet<DbOfficeUser> OfficesUsers { get; set; }
    public DbSet<DbWorkspace> Workspaces { get; set; }
    public DbSet<DbWorkspaceType> WorkspacesTypes { get; set; }
    public DbSet<DbWorkspaceImage> WorkspacesImages { get; set; }
    public DbSet<DbWorkspaceTag> WorkspacesTags { get; set; }

    public OfficeServiceDbContext(DbContextOptions<OfficeServiceDbContext> options)
      : base(options)
    {
    }

    // Fluent API is written here.\
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.ApplyConfigurationsFromAssembly(Assembly.Load("LT.DigitalOffice.OfficeService.Models.Db"));
    }
  }
}

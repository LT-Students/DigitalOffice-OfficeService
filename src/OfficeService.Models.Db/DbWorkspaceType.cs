using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LT.DigitalOffice.OfficeService.Models.Db
{
  public class DbWorkspaceType
  {
    public const string TableName = "WorkspacesTypes";

    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public int BookingRule { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public Guid? ModifiedBy { get; set; }

    public DateTime? ModifiedAtUtc { get; set; }

    public bool IsActive { get; set; }
  }

  public class DbWorkspaceTypeConfiguration : IEntityTypeConfiguration<DbWorkspaceType>
  {
    public void Configure(EntityTypeBuilder<DbWorkspaceType> builder)
    {
      builder
        .ToTable(DbWorkspaceType.TableName);

      builder
        .HasKey(wt => wt.Id);
    }
  }
}

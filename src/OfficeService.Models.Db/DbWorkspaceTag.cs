using System;

using LT.DigitalOffice.Kernel.BrokerSupport.Attributes.ParseEntity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LT.DigitalOffice.OfficeService.Models.Db
{
  public class DbWorkspaceTag
  {
    public const string TableName = "WorkspacesTags";

    public Guid Id { get; set; }

    public Guid WorkspaceId { get; set; }

    public Guid TagId { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    [IgnoreParse] 
    public DbWorkspace Workspace { get; set; }

    [IgnoreParse]
    public DbTag Tag { get; set; }
  }

  public class DbWorkspaceTagConfiguration : IEntityTypeConfiguration<DbWorkspaceTag>
  {
    public void Configure(EntityTypeBuilder<DbWorkspaceTag> builder)
    {
      builder
        .ToTable(DbWorkspaceTag.TableName);

      builder
        .HasKey(wt => wt.Id);

      builder
        .HasOne(wt => wt.Workspace)
        .WithMany(w => w.WorkspacesTags);

      builder
        .HasOne(wt => wt.Tag)
        .WithMany(t => t.WorkspacesTags);
    }
  }
}

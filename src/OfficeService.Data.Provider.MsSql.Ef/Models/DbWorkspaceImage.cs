using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LT.DigitalOffice.OfficeService.DataLayer.Models
{
  public class DbWorkspaceImage
  {
    public const string TableName = "WorkspacesImages";

    public Guid Id { get; set; }

    public Guid WorkspaceId { get; set; }

    public Guid ImageId { get; set; }

    public bool IsFrontImage { get; set; }

    public DbWorkspace Workspace { get; set; }
  }

  public class DbWorkspaceImageConfiguration : IEntityTypeConfiguration<DbWorkspaceImage>
  {
    public void Configure(EntityTypeBuilder<DbWorkspaceImage> builder)
    {
      builder
        .ToTable(DbWorkspaceImage.TableName);

      builder
        .HasKey(wi => wi.Id);

      builder
        .HasOne(wi => wi.Workspace)
        .WithMany(w => w.Images);
    }
  }
}

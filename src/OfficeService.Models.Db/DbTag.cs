using System;
using System.Collections.Generic;

using LT.DigitalOffice.Kernel.BrokerSupport.Attributes.ParseEntity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LT.DigitalOffice.OfficeService.Models.Db
{
  public class DbTag
  {
    public const string TableName = "Tags";

    public Guid Id { get; set; }

    public string Name { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    [IgnoreParse]
    public ICollection<DbWorkspaceTag> WorkspacesTags { get; set; }
  }

  public class DbTagConfiguration : IEntityTypeConfiguration<DbTag>
  {
    public void Configure(EntityTypeBuilder<DbTag> builder)
    {
      builder
        .ToTable(DbTag.TableName);

      builder
        .HasKey(e => e.Id);
    }
  }
}

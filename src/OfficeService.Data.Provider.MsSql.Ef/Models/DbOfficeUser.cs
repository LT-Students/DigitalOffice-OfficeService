using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LT.DigitalOffice.OfficeService.DataLayer.Models
{
  public class DbOfficeUser
  {
    public const string TableName = "OfficesUsers";

    public Guid Id { get; set; }
    public Guid OfficeId { get; set; }
    public Guid UserId { get; set; }
    public bool IsActive { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public Guid? ModifiedBy { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }

    public DbOffice Office { get; set; }
  }

  public class DbOfficeUserConfiguration : IEntityTypeConfiguration<DbOfficeUser>
  {
    public void Configure(EntityTypeBuilder<DbOfficeUser> builder)
    {
      builder
        .ToTable(DbOfficeUser.TableName);

      builder
        .HasKey(p => p.Id);

      builder
        .HasOne(pu => pu.Office)
        .WithMany(p => p.Users);
    }
  }
}

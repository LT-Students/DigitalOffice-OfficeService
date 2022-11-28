using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LT.DigitalOffice.OfficeService.DataLayer.Models
{
  public class DbOffice
  {
    public const string TableName = "Offices";

    public Guid Id { get; set; }
    public string Name { get; set; }
    public string City { get; set; }
    public string Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public Guid? ModifiedBy { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }
    public bool IsActive { get; set; }

    public ICollection<DbOfficeUser> Users { get; set; }

    public DbOffice()
    {
      Users = new HashSet<DbOfficeUser>();
    }
  }

  public class DbOfficeConfiguration : IEntityTypeConfiguration<DbOffice>
  {
    public void Configure(EntityTypeBuilder<DbOffice> builder)
    {
      builder
        .ToTable(DbOffice.TableName);

      builder
        .HasKey(o => o.Id);

      builder
        .Property(o => o.City)
        .IsRequired();

      builder
        .Property(o => o.Address)
        .IsRequired();

      builder
        .HasMany(o => o.Users)
        .WithOne(u => u.Office);
    }
  }
}

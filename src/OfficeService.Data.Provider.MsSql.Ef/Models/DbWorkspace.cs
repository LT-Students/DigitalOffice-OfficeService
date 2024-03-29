﻿using System;
using System.Collections.Generic;
using LT.DigitalOffice.Kernel.BrokerSupport.Attributes.ParseEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LT.DigitalOffice.OfficeService.DataLayer.Models
{
  public class DbWorkspace
  {
    public const string TableName = "Workspaces";

    public Guid Id { get; set; }

    public Guid? ParentId { get; set; }

    public string Name { get; set; }

    public Guid? WorkspaceTypeId { get; set; }

    public string Description { get; set; }

    public bool IsBookable { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public Guid? ModifiedBy { get; set; }

    public DateTime? ModifiedAtUtc { get; set; }

    public bool IsActive { get; set; }

    public DbWorkspaceType WorkspaceType { get; set; }

    [IgnoreParse]
    public ICollection<DbWorkspaceTag> WorkspacesTags { get; set; }

    [IgnoreParse]
    public ICollection<DbWorkspaceImage> Images { get; set; }

    public DbWorkspace()
    {
      WorkspacesTags = new List<DbWorkspaceTag>();
      Images = new List<DbWorkspaceImage>();
    }
  }

  public class DbWorkspaceConfiguration : IEntityTypeConfiguration<DbWorkspace>
  {
    public void Configure(EntityTypeBuilder<DbWorkspace> builder)
    {
      builder
        .ToTable(DbWorkspace.TableName);

      builder
        .HasKey(w => w.Id);

      builder
        .Property(w => w.Name)
        .IsRequired();

      builder
        .HasOne(w => w.WorkspaceType);

      builder
        .HasMany(w => w.WorkspacesTags)
        .WithOne(t => t.Workspace);

      builder
        .HasMany(w => w.Images)
        .WithOne(wi => wi.Workspace);
    }
  }
}

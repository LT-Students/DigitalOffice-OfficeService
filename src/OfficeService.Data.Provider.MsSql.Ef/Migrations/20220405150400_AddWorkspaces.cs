using System;

using LT.DigitalOffice.OfficeService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.OfficeService.Models.Db;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LT.DigitalOffice.OfficeService.Data.Migrations
{
  [DbContext(typeof(OfficeServiceDbContext))]
  [Migration("20220405150400_AddWorkspaces")]
  public class AddWorkspaces : Migration
  {
    #region Create tables

    private void CreateWorkspacesTable(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
        name: DbWorkspace.TableName,
        columns: table => new
        {
          Id = table.Column<Guid>(nullable: false),
          ParentId = table.Column<Guid>(nullable: true),
          Name = table.Column<string>(nullable: false),
          WorkspaceTypeId = table.Column<Guid>(nullable: false),
          Description = table.Column<string>(nullable: true),
          IsBookable = table.Column<bool>(nullable: false),
          CreatedBy = table.Column<Guid>(nullable: false),
          CreatedAtUtc = table.Column<DateTime>(nullable: false),
          ModifiedBy = table.Column<Guid>(nullable: true),
          ModifiedAtUtc = table.Column<DateTime>(nullable: true),
          IsActive = table.Column<bool>(nullable: false)
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_Workspaces", w => w.Id);
        });
    }

    private void CreateWorkspacesTypesTable(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
        name: DbWorkspaceType.TableName,
        columns: table => new
        {
          Id = table.Column<Guid>(nullable: false),
          Name = table.Column<string>(nullable: false),
          Description = table.Column<string>(nullable: true),
          StartTime = table.Column<TimeOnly>(nullable: true),
          EndTime = table.Column<TimeOnly>(nullable: true),
          BookingRule = table.Column<int>(nullable: false),
          CreatedBy = table.Column<Guid>(nullable: false),
          CreatedAtUtc = table.Column<DateTime>(nullable: false),
          ModifiedBy = table.Column<Guid>(nullable: true),
          ModifiedAtUtc = table.Column<DateTime>(nullable: true),
          IsActive = table.Column<bool>(nullable: false)
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_WorkspacesTypes", w => w.Id);
        });
    }

    private void CreateTagsTable(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
        name: DbTag.TableName,
        columns: table => new
        {
          Id = table.Column<Guid>(nullable: false),
          Name = table.Column<string>(nullable: false),
          CreatedBy = table.Column<Guid>(nullable: false),
          CreatedAtUtc = table.Column<DateTime>(nullable: false),
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_Tags", w => w.Id);
        });
    }

    private void CreateWorkspacesTagsTable(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
        name: DbWorkspaceTag.TableName,
        columns: table => new
        {
          Id = table.Column<Guid>(nullable: false),
          WorkspaceID = table.Column<Guid>(nullable: false),
          TagId = table.Column<Guid>(nullable: false),
          CreatedBy = table.Column<Guid>(nullable: false),
          CreatedAtUtc = table.Column<DateTime>(nullable: false),
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_WorkspacesTags", w => w.Id);
        });
    }

    private void CreateWorkspacesImagesTable(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
        name: DbWorkspaceImage.TableName,
        columns: table => new
        {
          Id = table.Column<Guid>(nullable: false),
          WorkspaceID = table.Column<Guid>(nullable: false),
          ImageId = table.Column<Guid>(nullable: false),
          IsFrontImage = table.Column<bool>(nullable: false),
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_WorkspacesImages", w => w.Id);
        });
    }

    #endregion

    protected override void Up(MigrationBuilder migrationBuilder)
    {
      CreateWorkspacesTable(migrationBuilder);
      CreateWorkspacesTypesTable(migrationBuilder);
      CreateTagsTable(migrationBuilder);
      CreateWorkspacesTagsTable(migrationBuilder);
      CreateWorkspacesImagesTable(migrationBuilder);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(DbWorkspace.TableName);
      migrationBuilder.DropTable(DbWorkspaceType.TableName);
      migrationBuilder.DropTable(DbTag.TableName);
      migrationBuilder.DropTable(DbWorkspaceTag.TableName);
      migrationBuilder.DropTable(DbWorkspaceImage.TableName);
    }
  }
}

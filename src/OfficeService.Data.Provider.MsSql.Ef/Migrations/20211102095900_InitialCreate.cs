using System;
using LT.DigitalOffice.OfficeService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.OfficeService.Models.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LT.DigitalOffice.OfficeService.Data.Migrations
{
  [DbContext(typeof(OfficeServiceDbContext))]
  [Migration("20211102095900_InitialCreate")]
  public class InitialCreate : Migration
  {
    #region Create tables

    private void CreateOfficesTable(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
        name: DbOffice.TableName,
        columns: table => new
        {
          Id = table.Column<Guid>(nullable: false),
          Name = table.Column<string>(nullable: true),
          City = table.Column<string>(nullable: false),
          Address = table.Column<string>(nullable: false),
          Latitude = table.Column<double>(nullable: true),
          Longitude = table.Column<double>(nullable: true),
          CreatedBy = table.Column<Guid>(nullable: false),
          CreatedAtUtc = table.Column<DateTime>(nullable: false),
          ModifiedBy = table.Column<Guid>(nullable: true),
          ModifiedAtUtc = table.Column<DateTime>(nullable: true),
          IsActive = table.Column<bool>(nullable: false)
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_Offices", x => x.Id);
        });
    }

    private void CreateOfficesUsersTable(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
        name: DbOfficeUser.TableName,
        columns: table => new
        {
          Id = table.Column<Guid>(nullable: false),
          OfficeId = table.Column<Guid>(nullable: false),
          UserId = table.Column<Guid>(nullable: false),
          CreatedBy = table.Column<Guid>(nullable: false),
          CreatedAtUtc = table.Column<DateTime>(nullable: false),
          ModifiedBy = table.Column<Guid>(nullable: true),
          ModifiedAtUtc = table.Column<DateTime>(nullable: true),
          IsActive = table.Column<bool>(nullable: false)
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_OfficesUsers", x => x.Id);
        });
    }

    #endregion

    protected override void Up(MigrationBuilder migrationBuilder)
    {
      CreateOfficesTable(migrationBuilder);
      CreateOfficesUsersTable(migrationBuilder);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(DbOffice.TableName);
      migrationBuilder.DropTable(DbOfficeUser.TableName);
    }
  }
}

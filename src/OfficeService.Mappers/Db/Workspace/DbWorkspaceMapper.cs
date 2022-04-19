using System;
using System.Text.RegularExpressions;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.OfficeService.Mappers.Db.Workspace.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Db;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Workspace;
using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.OfficeService.Mappers.Db.Workspace
{
  public class DbWorkspaceMapper : IDbWorkspaceMapper
  {
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly Regex _nameRegex = new(@"^\s+|\s+$|\s+(?=\s)");

    public DbWorkspaceMapper(IHttpContextAccessor httpContextAccessor)
    {
      _httpContextAccessor = httpContextAccessor;
    }

    public DbWorkspace Map(CreateWorkspaceRequest request)
    {
      if (request is null)
      {
        return null;
      }

      return new DbWorkspace
      {
        Id = Guid.NewGuid(),
        ParentId = request.ParentId,
        Name = !string.IsNullOrWhiteSpace(request.Name) ? _nameRegex.Replace(request.Name, "") : null,
        WorkspaceTypeId = request.WorkspaceTypeId,
        Description = request.Description.Trim(),
        IsBookable = request.IsBookable,
        CreatedAtUtc = DateTime.UtcNow,
        CreatedBy = _httpContextAccessor.HttpContext.GetUserId(),
        IsActive = true
      };
    }
  }
}

using System;
using System.Text.RegularExpressions;

using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.OfficeService.Mappers.Db.WorkspaceType.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Db;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.WorkspaceType;

using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.OfficeService.Mappers.Db.WorkspaceType
{
  public class DbWorkspaceTypeMapper : IDbWorkspaceTypeMapper
  {
    private readonly IHttpContextAccessor _httpContextAccessor;
    // TODO: Check the regex
    private readonly Regex _nameRegex = new(@"^\s+|\s+$|\s+(?=\s)");

    public DbWorkspaceTypeMapper(IHttpContextAccessor httpContextAccessor)
    {
      _httpContextAccessor = httpContextAccessor;
    }

    public DbWorkspaceType Map(CreateWorkspaceTypeRequest request)
    {
      if (request is null)
      {
        return null;
      }

      return new DbWorkspaceType
      {
        Id = Guid.NewGuid(),
        Name = !string.IsNullOrWhiteSpace(request.Name) ? _nameRegex.Replace(request.Name, "") : null,
        Description = request.Description.Trim(),
        StartTime = request.StartTime,
        EndTime = request.EndTime,
        BookingRule = (int)request.BookingRule,
        CreatedAtUtc = DateTime.UtcNow,
        CreatedBy = _httpContextAccessor.HttpContext.GetUserId(),
        IsActive = true
      };
    }
  }
}

using System;
using System.Linq;
using System.Text.RegularExpressions;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.OfficeService.Mappers.Db.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Db;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Office;
using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.OfficeService.Mappers.Db
{
  public class DbOfficeMapper : IDbOfficeMapper
  {
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly Regex _nameRegex = new(@"^\s+|\s+$|\s+(?=\s)");

    public DbOfficeMapper(IHttpContextAccessor httpContextAccessor)
    {
      _httpContextAccessor = httpContextAccessor;
    }

    public DbOffice Map(CreateOfficeRequest request)
    {
      if (request == null)
      {
        return null;
      }

      return new DbOffice
      {
        Id = Guid.NewGuid(),
        Name = !string.IsNullOrWhiteSpace(request.Name) ? _nameRegex.Replace(request.Name, "") : null,
        City = request.City.Trim(),
        Address = request.Address.Trim(),
        Latitude = request.Latitude,
        Longitude = request.Longitude,
        CreatedAtUtc = DateTime.UtcNow,
        CreatedBy = _httpContextAccessor.HttpContext.GetUserId(),
        IsActive = true
      };
    }
  }
}

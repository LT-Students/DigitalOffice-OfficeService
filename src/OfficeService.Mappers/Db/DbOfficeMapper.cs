using System;
using System.Linq;
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
        Name = request.Name != null && request.Name.Trim().Any() ? request.Name.Trim() : null,
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

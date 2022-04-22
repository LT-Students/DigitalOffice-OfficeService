using System;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Models.Broker.Publishing.Subscriber.Office;
using LT.DigitalOffice.OfficeService.Mappers.Db.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Db;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Users;
using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.OfficeService.Mappers.Db
{
  public class DbOfficeUserMapper : IDbOfficeUserMapper
  {
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DbOfficeUserMapper(IHttpContextAccessor httpContextAccessor)
    {
      _httpContextAccessor = httpContextAccessor;
    }

    public DbOfficeUser Map(ICreateUserOfficePublish request)
    {
      if (request == null)
      {
        return null;
      }

      return new DbOfficeUser
      {
        Id = Guid.NewGuid(),
        OfficeId = request.OfficeId,
        UserId = request.UserId,
        CreatedAtUtc = DateTime.UtcNow,
        CreatedBy = request.CreatedBy,
        IsActive = true
      };
    }

    public DbOfficeUser Map(ChangeUserOfficeRequest request)
    {
      if (request == null || !request.OfficeId.HasValue)
      {
        return null;
      }

      return new DbOfficeUser
      {
        Id = Guid.NewGuid(),
        OfficeId = request.OfficeId.Value,
        UserId = request.UserId,
        CreatedAtUtc = DateTime.UtcNow,
        CreatedBy = _httpContextAccessor.HttpContext.GetUserId(),
        IsActive = true
      };
    }
  }
}

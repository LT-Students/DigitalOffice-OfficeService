using System;
using System.Collections.Generic;
using LT.DigitalOffice.Kernel.Extensions;
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

    public List<DbOfficeUser> Map(CreateOfficeUsers request)
    {
      return request?.UsersIds.ConvertAll(u => new DbOfficeUser
      {
        Id = Guid.NewGuid(),
        OfficeId = request.OfficeId,
        UserId = u,
        CreatedAtUtc = DateTime.UtcNow,
        CreatedBy = _httpContextAccessor.HttpContext.GetUserId(),
        IsActive = true
      });
    }
  }
}

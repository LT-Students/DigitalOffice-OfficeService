using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.OfficeService.Data.Provider;
using LT.DigitalOffice.OfficeService.Models.Db;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.OfficeService.Business.Commands.Users.Create
{
  public class CreateOfficeUsersHandler : IRequestHandler<CreateOfficeUsersRequest, bool>
  {
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IGlobalCacheRepository _globalCache;
    private readonly IDataProvider _provider;

    #region private methods

    private async Task<List<Guid>> RemoveOfficeUsersAsync(List<Guid> usersIds, Guid? officeId, CancellationToken ct)
    {
      if (usersIds is null || !usersIds.Any())
      {
        return null;
      }

      List<DbOfficeUser> officeUsers = officeId.HasValue
        ? await _provider.OfficesUsers
          .Where(ou => ou.OfficeId == officeId && usersIds.Contains(ou.UserId))
          .ToListAsync(ct)
        : await _provider.OfficesUsers
          .Where(ou => usersIds.Contains(ou.UserId))
          .ToListAsync(ct);

      _provider.OfficesUsers.RemoveRange(officeUsers);
      await _provider.SaveAsync();

      return officeUsers.Select(ou => ou.UserId).ToList();
    }

    private async Task<bool> CreateOfficesUsersAsync(List<DbOfficeUser> dbOfficesUsers)
    {
      if (dbOfficesUsers is null)
      {
        return false;
      }

      await _provider.OfficesUsers.AddRangeAsync(dbOfficesUsers);
      await _provider.SaveAsync();

      return true;
    }

    private List<DbOfficeUser> Map(CreateOfficeUsersRequest request)
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

    #endregion

    public CreateOfficeUsersHandler(
      IHttpContextAccessor httpContextAccessor,
      IGlobalCacheRepository globalCache,
      IDataProvider provider)
    {
      _httpContextAccessor = httpContextAccessor;
      _globalCache = globalCache;
      _provider = provider;
    }

    public async Task<bool> Handle(CreateOfficeUsersRequest request, CancellationToken ct)
    {
      List<Guid> removedUsersIds = await RemoveOfficeUsersAsync(request.UsersIds, null, ct);
      if (removedUsersIds is not null)
      {
        foreach (Guid removedUserId in removedUsersIds)
        {
          await _globalCache.RemoveAsync(removedUserId);
        }
      }

      return await CreateOfficesUsersAsync(Map(request));
    }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.OfficeService.Data.Provider;
using LT.DigitalOffice.OfficeService.DataLayer.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.OfficeService.Business.Users.Remove
{
  public class RemoveOfficeUsersHandler : IRequestHandler<RemoveOfficeUsersRequest, bool>
  {
    private readonly IDataProvider _provider;
    private readonly IGlobalCacheRepository _globalCache;

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

    #endregion

    public RemoveOfficeUsersHandler(
      IDataProvider provider,
      IGlobalCacheRepository globalCache)
    {
      _provider = provider;
      _globalCache = globalCache;
    }

    public async Task<bool> Handle(RemoveOfficeUsersRequest request, CancellationToken ct)
    {
      List<Guid> removedUsersIds = await RemoveOfficeUsersAsync(request.UsersIds, request.OfficeId, ct);
      if (!removedUsersIds.Any())
      {
        return false;
      }

      foreach (Guid removedUserId in removedUsersIds)
      {
        await _globalCache.RemoveAsync(removedUserId);
      }

      return true;
    }
  }
}

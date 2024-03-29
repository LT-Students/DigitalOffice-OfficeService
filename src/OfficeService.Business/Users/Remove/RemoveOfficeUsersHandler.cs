﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.OfficeService.DataLayer;
using LT.DigitalOffice.OfficeService.DataLayer.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.OfficeService.Business.Users.Remove
{
  public class RemoveOfficeUsersHandler : IRequestHandler<RemoveOfficeUsersRequest, bool>
  {
    private readonly OfficeServiceDbContext _dbContext;
    private readonly IGlobalCacheRepository _globalCache;

    #region private methods

    private async Task<List<Guid>> RemoveOfficeUsersAsync(List<Guid> usersIds, Guid? officeId, CancellationToken ct)
    {
      if (usersIds is null || !usersIds.Any())
      {
        return null;
      }

      List<DbOfficeUser> officeUsers = officeId.HasValue
        ? await _dbContext.OfficesUsers
          .Where(ou => ou.OfficeId == officeId && usersIds.Contains(ou.UserId))
          .ToListAsync(ct)
        : await _dbContext.OfficesUsers
          .Where(ou => usersIds.Contains(ou.UserId))
          .ToListAsync(ct);

      _dbContext.OfficesUsers.RemoveRange(officeUsers);
      await _dbContext.SaveAsync();

      return officeUsers.Select(ou => ou.UserId).ToList();
    }

    #endregion

    public RemoveOfficeUsersHandler(
      OfficeServiceDbContext dbContext,
      IGlobalCacheRepository globalCache)
    {
      _dbContext = dbContext;
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

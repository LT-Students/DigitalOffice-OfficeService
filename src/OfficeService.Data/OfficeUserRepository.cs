﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.OfficeService.Data.Interfaces;
using LT.DigitalOffice.OfficeService.Data.Provider;
using LT.DigitalOffice.OfficeService.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.OfficeService.Data
{
  public class OfficeUserRepository : IOfficeUserRepository
  {
    private readonly IDataProvider _provider;

    public OfficeUserRepository(IDataProvider provider)
    {
      _provider = provider;
    }

    public async Task<bool> CreateAsync(DbOfficeUser user)
    {
      if (user == null)
      {
        return false;
      }

      _provider.OfficesUsers.Add(user);
      await _provider.SaveAsync();

      return true;
    }

    public async Task<List<DbOfficeUser>> GetAsync(List<Guid> usersIds)
    {
      IQueryable<DbOfficeUser> users = _provider.OfficesUsers.Include(ou => ou.Office).AsQueryable();

      if (usersIds != null)
      {
        users = users.Where(x => usersIds.Contains(x.UserId) && x.IsActive);
      }

      return await users.ToListAsync();
    }

    public async Task<List<DbOfficeUser>> GetOfficeAsync(List<Guid> officesIds)
    {
      return await _provider.OfficesUsers.Include(x => x.Office).Where(
        u => u.IsActive &&
        officesIds.Contains(u.OfficeId))
        .ToListAsync();
    }

    public async Task<Guid?> RemoveAsync(Guid userId, Guid removedBy)
    {
      DbOfficeUser user = await _provider.OfficesUsers.FirstOrDefaultAsync(u => u.UserId == userId && u.IsActive);

      if (user != null)
      {
        user.IsActive = false;
        user.ModifiedAtUtc = DateTime.UtcNow;
        user.ModifiedBy = removedBy;
        await _provider.SaveAsync();

        return user.OfficeId;
      }

      return null;
    }
  }
}

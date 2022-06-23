using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.OfficeService.Data.Interfaces;
using LT.DigitalOffice.OfficeService.Data.Provider;
using LT.DigitalOffice.OfficeService.Models.Db;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.OfficeService.Data
{
  public class OfficeUserRepository : IOfficeUserRepository
  {
    private readonly IDataProvider _provider;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public OfficeUserRepository(
      IDataProvider provider,
      IHttpContextAccessor httpContextAccessor)
    {
      _provider = provider;
      _httpContextAccessor = httpContextAccessor;
    }

    public async Task<bool> CreateAsync(DbOfficeUser dbOfficeUser)
    {
      if (dbOfficeUser is null)
      {
        return false;
      }

      _provider.OfficesUsers.Add(dbOfficeUser);
      await _provider.SaveAsync();

      return true;
    }

    public async Task<bool> CreateAsync(List<DbOfficeUser> dbOfficesUsers)
    {
      if (dbOfficesUsers is null)
      {
        return false;
      }

      await _provider.OfficesUsers.AddRangeAsync(dbOfficesUsers);
      await _provider.SaveAsync();

      return true;
    }

    public async Task<List<DbOfficeUser>> GetAsync(List<Guid> usersIds)
    {
      IQueryable<DbOfficeUser> users = _provider.OfficesUsers.Include(ou => ou.Office).AsQueryable();

      if (usersIds is not null)
      {
        users = users.Where(x => usersIds.Contains(x.UserId) && x.IsActive);
      }

      return await users.ToListAsync();
    }

    public async Task<List<DbOfficeUser>> GetAsync(List<Guid> usersIds, Guid officeId)
    {
      IQueryable<DbOfficeUser> users = _provider.OfficesUsers
        .Where(ou => ou.OfficeId == officeId && usersIds.Contains(ou.UserId)) 
        .Include(ou => ou.Office)
        .AsQueryable();

      return await users.ToListAsync();
    }

    public async Task<Guid?> RemoveAsync(Guid userId, Guid removedBy)
    {
      DbOfficeUser user = await _provider.OfficesUsers.FirstOrDefaultAsync(u => u.UserId == userId && u.IsActive);

      if (user is not null)
      {
        user.IsActive = false;
        user.ModifiedAtUtc = DateTime.UtcNow;
        user.ModifiedBy = removedBy;
        await _provider.SaveAsync();

        return user.OfficeId;
      }

      return null;
    }

    public async Task<bool> RemoveAsync(Guid officeId)
    {
      List<DbOfficeUser> dbUsers = await _provider.OfficesUsers.Where(x => x.OfficeId == officeId).ToListAsync();
      DateTime modifiedAtUtc = DateTime.UtcNow;
      Guid senderId = _httpContextAccessor.HttpContext.GetUserId();

      foreach (DbOfficeUser user in dbUsers)
      {
        user.IsActive = false;
        user.ModifiedAtUtc = modifiedAtUtc;
        user.ModifiedBy = senderId;
      }

      await _provider.SaveAsync();

      return true;
    }

    public async Task<bool> RemoveAsync(List<Guid> usersIds, Guid officeId)
    {
      List<DbOfficeUser> officeUsers = await _provider.OfficesUsers
        .Where(ou => ou.OfficeId == officeId && usersIds.Contains(ou.UserId))
        .ToListAsync();

      _provider.OfficesUsers.RemoveRange(officeUsers);
      await _provider.SaveAsync();

      return true;
    }
  }
}

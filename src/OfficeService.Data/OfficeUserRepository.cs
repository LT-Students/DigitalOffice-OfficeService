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

    public async Task<Guid?> CreateAsync(DbOfficeUser dbOfficeUser)
    {
      if (dbOfficeUser is null)
      {
        return default;
      }

      _provider.OfficesUsers.Add(dbOfficeUser);
      await _provider.SaveAsync();

      return dbOfficeUser.Id;
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
  }
}

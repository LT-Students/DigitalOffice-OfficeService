using System;
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

    public async Task<List<DbOfficeUser>> GetAsync(List<Guid> usersIds)
    {
      IQueryable<DbOfficeUser> users = _provider.OfficesUsers
        .Where(u => usersIds.Contains(u.UserId) && u.IsActive)
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
  }
}

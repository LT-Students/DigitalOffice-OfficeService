using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.OfficeService.Models.Db;
using LT.DigitalOffice.Kernel.Attributes;

namespace LT.DigitalOffice.OfficeService.Data.Interfaces
{
  [AutoInject]
  public interface IOfficeUserRepository
  {
    Task<bool> CreateAsync(DbOfficeUser user);

    Task<List<DbOfficeUser>> GetAsync(List<Guid> usersIds);

    Task<Guid?> RemoveAsync(Guid userId, Guid removedBy);

    Task<bool> RemoveAsync(Guid officeId);
  }
}

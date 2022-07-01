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
    Task<bool> CreateAsync(DbOfficeUser dbOfficeUser);

    Task<bool> CreateAsync(List<DbOfficeUser> dbOfficesUsers);

    Task<List<DbOfficeUser>> GetAsync(List<Guid> usersIds);

    Task<List<DbOfficeUser>> GetAsync(List<Guid> usersIds, Guid officeId);

    Task<Guid?> RemoveAsync(Guid userId, Guid removedBy);

    Task<bool> RemoveAsync(Guid officeId);

    Task<List<Guid>> RemoveAsync(List<Guid> usersIds, Guid? officeId);
  }
}

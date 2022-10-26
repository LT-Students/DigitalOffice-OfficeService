using System;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Models.Broker.Publishing;
using LT.DigitalOffice.OfficeService.DataLayer;
using LT.DigitalOffice.OfficeService.DataLayer.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.OfficeService.Broker.Consumers
{
  public class DisactivateOfficeUserConsumer : IConsumer<IDisactivateUserPublish>
  {
    private readonly OfficeServiceDbContext _dbContext;
    private readonly IGlobalCacheRepository _globalCache;

    private async Task<Guid?> RemoveOfficeUsersAsync(Guid userId, Guid removedBy)
    {
      DbOfficeUser user = await _dbContext.OfficesUsers.FirstOrDefaultAsync(u => u.UserId == userId && u.IsActive);

      if (user is not null)
      {
        user.IsActive = false;
        user.ModifiedAtUtc = DateTime.UtcNow;
        user.ModifiedBy = removedBy;
        await _dbContext.SaveAsync();

        return user.OfficeId;
      }

      return null;
    }

    public DisactivateOfficeUserConsumer(
      OfficeServiceDbContext dbContext,
      IGlobalCacheRepository globalCache)
    {
      _dbContext = dbContext;
      _globalCache = globalCache;
    }

    public async Task Consume(ConsumeContext<IDisactivateUserPublish> context)
    {
      Guid? officeId = await RemoveOfficeUsersAsync(context.Message.UserId, context.Message.ModifiedBy);

      if (officeId.HasValue)
      {
        await _globalCache.RemoveAsync(officeId.Value);
      }
    }
  }
}

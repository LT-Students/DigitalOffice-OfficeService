using System.Threading.Tasks;
using DigitalOffice.Models.Broker.Publishing;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.OfficeService.DataLayer;
using LT.DigitalOffice.OfficeService.DataLayer.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.OfficeService.Broker.Consumers
{
  public class ActivateOfficeUserConsumer : IConsumer<IActivateUserPublish>
  {
    private readonly IGlobalCacheRepository _globalCache;
    private readonly OfficeServiceDbContext _dbContext;
    private readonly ILogger<ActivateOfficeUserConsumer> _logger;

    public ActivateOfficeUserConsumer(
      IGlobalCacheRepository globalCache,
      OfficeServiceDbContext dbContext,
      ILogger<ActivateOfficeUserConsumer> logger)
    {
      _globalCache = globalCache;
      _dbContext = dbContext;
      _logger = logger;
    }

    public async Task Consume(ConsumeContext<IActivateUserPublish> context)
    {
      DbOfficeUser user = await _dbContext.OfficesUsers
        .FirstOrDefaultAsync(ou => ou.Id == context.Message.UserId);

      if (user is not null)
      {
        user.IsActive = true;
        await _dbContext.SaveAsync();

        await _globalCache.RemoveAsync(user.Id);

        _logger.LogInformation($"User with Id {context.Message.UserId} activated in office with Id {user.OfficeId}");
      }
      else
      {
        _logger.LogInformation($"Cannot activate user with Id {context.Message.UserId}");
      }
    }
  }
}

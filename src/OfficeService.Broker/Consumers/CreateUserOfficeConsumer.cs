using System;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Models.Broker.Publishing.Subscriber.Office;
using LT.DigitalOffice.OfficeService.DataLayer;
using LT.DigitalOffice.OfficeService.DataLayer.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.OfficeService.Broker.Consumers
{
  public class CreateUserOfficeConsumer : IConsumer<ICreateUserOfficePublish>
  {
    private readonly IDataProvider _provider;
    private readonly IGlobalCacheRepository _globalCache;
    private readonly ILogger<CreateUserOfficeConsumer> _logger;

    public CreateUserOfficeConsumer(
      IDataProvider provider,
      IGlobalCacheRepository globalCache,
      ILogger<CreateUserOfficeConsumer> logger)
    {
      _provider = provider;
      _globalCache = globalCache;
      _logger = logger;
    }

    public async Task Consume(ConsumeContext<ICreateUserOfficePublish> context)
    {
      ICreateUserOfficePublish publish = context.Message;
      if (await _provider.Offices.AnyAsync(o => o.Id == publish.OfficeId))
      {
        if (publish is null)
        {
          return;
        }

        DbOfficeUser user = new DbOfficeUser
        {
          Id = Guid.NewGuid(),
          OfficeId = publish.OfficeId,
          UserId = publish.UserId,
          CreatedAtUtc = DateTime.UtcNow,
          CreatedBy = publish.CreatedBy,
          IsActive = publish.IsActive
        };

        await _provider.OfficesUsers.AddAsync(user);

        if (publish.IsActive)
        {
          await _globalCache.RemoveAsync(publish.OfficeId);
        }
      }
      else
      {
        _logger.LogError($"Cannot create user with Id {publish.UserId} in office with Id {publish.OfficeId}");
      }
    }
  }
}

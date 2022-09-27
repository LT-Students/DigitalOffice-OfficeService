using System;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Models.Broker.Publishing;
using LT.DigitalOffice.OfficeService.Data.Interfaces;
using MassTransit;

namespace LT.DigitalOffice.OfficeService.Broker.Consumers
{
  public class DisactivateOfficeUserConsumer : IConsumer<IDisactivateUserPublish>
  {
    private readonly IOfficeUserRepository _officeRepository;
    private readonly IGlobalCacheRepository _globalCache;

    public DisactivateOfficeUserConsumer(
      IOfficeUserRepository officeRepository,
      IGlobalCacheRepository globalCache)
    {
      _officeRepository = officeRepository;
      _globalCache = globalCache;
    }

    public async Task Consume(ConsumeContext<IDisactivateUserPublish> context)
    {
      Guid? officeId = await _officeRepository.RemoveAsync(context.Message.UserId, context.Message.ModifiedBy);

      if (officeId.HasValue)
      {
        await _globalCache.RemoveAsync(officeId.Value);
      }
    }
  }
}

using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Models.Broker.Publishing.Subscriber.Office;
using LT.DigitalOffice.OfficeService.Data.Interfaces;
using LT.DigitalOffice.OfficeService.Mappers.Db.Interfaces;
using MassTransit;

namespace LT.DigitalOffice.OfficeService.Broker.Consumers
{
  public class CreateUserOfficeConsumer : IConsumer<ICreateUserOfficePublish>
  {
    private readonly IOfficeRepository _officeRepository;
    private readonly IOfficeUserRepository _officeUserRepository;
    private readonly IDbOfficeUserMapper _officeUserMapper;
    private readonly IGlobalCacheRepository _globalCache;

    public CreateUserOfficeConsumer(
      IOfficeRepository officeRepository,
      IOfficeUserRepository officeUserRepository,
      IDbOfficeUserMapper officeUserMapper,
      IGlobalCacheRepository globalCache)
    {
      _officeRepository = officeRepository;
      _officeUserRepository = officeUserRepository;
      _officeUserMapper = officeUserMapper;
      _globalCache = globalCache;
    }

    public async Task Consume(ConsumeContext<ICreateUserOfficePublish> context)
    {
      if (await _officeRepository.DoesExistAsync(context.Message.OfficeId))
      {
        await _officeUserRepository.CreateAsync(_officeUserMapper.Map(context.Message));
        await _globalCache.RemoveAsync(context.Message.OfficeId);
      }
    }
  }
}

using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Models.Broker.Requests.Office;
using LT.DigitalOffice.OfficeService.Data.Interfaces;
using LT.DigitalOffice.OfficeService.Mappers.Db.Interfaces;
using MassTransit;

namespace LT.DigitalOffice.OfficeService.Broker.Consumers
{
  public class CreateUserOfficeConsumer : IConsumer<ICreateUserOfficeRequest>
  {
    private readonly IOfficeRepository _officeRepository;
    private readonly IOfficeUserRepository _officeUserRepository;
    private readonly IDbOfficeUserMapper _officeUserMapper;
    private readonly ICacheNotebook _cacheNotebook;

    private async Task<bool> CreateUserOffice(ICreateUserOfficeRequest request)
    {
      if (!await _officeRepository.DoesExistAsync(request.OfficeId))
      {
        return false;
      }

      return await _officeUserRepository.CreateAsync(_officeUserMapper.Map(request));
    }

    public CreateUserOfficeConsumer(
      IOfficeRepository officeRepository,
      IOfficeUserRepository officeUserRepository,
      IDbOfficeUserMapper officeUserMapper,
      ICacheNotebook cacheNotebook)
    {
      _officeRepository = officeRepository;
      _officeUserRepository = officeUserRepository;
      _officeUserMapper = officeUserMapper;
      _cacheNotebook = cacheNotebook;
    }

    public async Task Consume(ConsumeContext<ICreateUserOfficeRequest> context)
    {
      bool result = await CreateUserOffice(context.Message);

      object response = OperationResultWrapper.CreateResponse((_) => result, context.Message);

      await context.RespondAsync<IOperationResult<bool>>(response);

      if (result)
      {
        await _cacheNotebook.RemoveAsync(context.Message.OfficeId);
      }
    }
  }
}

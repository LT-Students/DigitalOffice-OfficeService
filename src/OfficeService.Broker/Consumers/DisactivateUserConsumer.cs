using System.Threading.Tasks;
using LT.DigitalOffice.OfficeService.Data.Interfaces;
using LT.DigitalOffice.Models.Broker.Common;
using MassTransit;

namespace LT.DigitalOffice.OfficeService.Broker.Consumers
{
  public class DisactivateUserConsumer : IConsumer<IDisactivateUserRequest>
  {
    private readonly IOfficeUserRepository _officeRepository;

    public DisactivateUserConsumer(
      IOfficeUserRepository officeRepository)
    {
      _officeRepository = officeRepository;
    }

    public async Task Consume(ConsumeContext<IDisactivateUserRequest> context)
    {
      await _officeRepository.RemoveAsync(context.Message.UserId, context.Message.ModifiedBy);
    }
  }
}

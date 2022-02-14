using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
using LT.DigitalOffice.Models.Broker.Models.Office;
using LT.DigitalOffice.Models.Broker.Requests.Office;
using LT.DigitalOffice.Models.Broker.Responses.Office;
using LT.DigitalOffice.OfficeService.Data.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Db;
using MassTransit;

namespace LT.DigitalOffice.OfficeService.Broker.Consumers
{
  public class FilterOfficesUsersConsumer : IConsumer<IFilterOfficesRequest>
  {
    private readonly IOfficeUserRepository _repository;

    private async Task<List<OfficeFilteredData>> GetOfficesDataAsync(IFilterOfficesRequest request)
    {
      List<DbOffice> offices = (await _repository
        .GetOfficeAsync(request.OfficesIds))
        .Select(du => du.Office)
        .Distinct()
        .ToList();

      return offices.Select(o =>
          new OfficeFilteredData(
            o.Id,
            o.Name,
            o.Users.Where(u => u.OfficeId == o.Id).Select(u => u.UserId).ToList()))
        .ToList();
    }
    public FilterOfficesUsersConsumer(
      IOfficeUserRepository repository)
    {
      _repository = repository;
    }

    public async Task Consume(ConsumeContext<IFilterOfficesRequest> context)
    {
      List<OfficeFilteredData> officesFilteredData = await GetOfficesDataAsync(context.Message);

      await context.RespondAsync<IOperationResult<IFilterOfficesResponse>>(
        OperationResultWrapper.CreateResponse((_) => IFilterOfficesResponse.CreateObj(officesFilteredData), context));

    }
  }
}

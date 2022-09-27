using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
using LT.DigitalOffice.Kernel.RedisSupport.Configurations;
using LT.DigitalOffice.Kernel.RedisSupport.Constants;
using LT.DigitalOffice.Kernel.RedisSupport.Extensions;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Models.Broker.Models.Office;
using LT.DigitalOffice.Models.Broker.Requests.Office;
using LT.DigitalOffice.Models.Broker.Responses.Office;
using LT.DigitalOffice.OfficeService.Data.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Db;
using MassTransit;
using Microsoft.Extensions.Options;

namespace LT.DigitalOffice.OfficeService.Broker.Consumers
{
  public class FilterOfficesUsersConsumer : IConsumer<IFilterOfficesRequest>
  {
    private readonly IOfficeRepository _repository;
    private readonly IOptions<RedisConfig> _redisConfig;
    private readonly IGlobalCacheRepository _globalCache;

    private async Task<List<OfficeFilteredData>> GetOfficesDataAsync(IFilterOfficesRequest request)
    {
      List<DbOffice> offices = await _repository.GetAsync(request.OfficesIds);

      return offices.Select(
        o => new OfficeFilteredData(
          o.Id,
          o.Name,
          o.Users.Select(u => u.UserId).ToList()))
        .ToList();
    }
    public FilterOfficesUsersConsumer(
      IOfficeRepository repository,
      IOptions<RedisConfig> redisConfig,
      IGlobalCacheRepository globalCache)
    {
      _repository = repository;
      _redisConfig = redisConfig;
      _globalCache = globalCache;
    }

    public async Task Consume(ConsumeContext<IFilterOfficesRequest> context)
    {
      List<OfficeFilteredData> officesFilteredData = await GetOfficesDataAsync(context.Message);

      await context.RespondAsync<IOperationResult<IFilterOfficesResponse>>(
        OperationResultWrapper.CreateResponse((_) => IFilterOfficesResponse.CreateObj(officesFilteredData), context));

      if (officesFilteredData is not null)
      {
        await _globalCache.CreateAsync(
          Cache.Offices,
          context.Message.OfficesIds.GetRedisCacheKey(context.Message.GetBasicProperties()),
          officesFilteredData,
          context.Message.OfficesIds,
          TimeSpan.FromMinutes(_redisConfig.Value.CacheLiveInMinutes));
      }
    }
  }
}

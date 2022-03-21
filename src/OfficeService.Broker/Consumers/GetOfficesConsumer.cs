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
  public class GetOfficesConsumer : IConsumer<IGetOfficesRequest>
  {
    private readonly IOfficeUserRepository _officeUserRepository;
    private readonly IOptions<RedisConfig> _redisConfig;
    private readonly IGlobalCacheRepository _globalCache;

    private async Task<List<OfficeData>> GetOfficesAsync(List<Guid> userIds)
    {
      List<DbOffice> offices = (await _officeUserRepository
        .GetAsync(userIds))
        .Select(du => du.Office)
        .Distinct()
        .ToList();

      return offices.Select(
        o => new OfficeData(
          o.Id,
          o.Name,
          o.City,
          o.Name,
          o.Latitude ?? default,
          o.Longitude ?? default,
          o.Users.Select(u => u.UserId).ToList())).ToList();
    }

    private async Task CreateCache(
      List<Guid> userIds,
      List<OfficeData> offices)
    {
      if (userIds == null)
      {
        return;
      }

      string key = userIds.GetRedisCacheHashCode();

      if (offices != null && offices.Any())
      {
        await _globalCache.CreateAsync(
          Cache.Offices,
          key,
          offices,
          offices.Select(o => o.Id).ToList(),
          TimeSpan.FromMinutes(_redisConfig.Value.CacheLiveInMinutes));
      }
    }

    public GetOfficesConsumer(
      IOfficeUserRepository officeUserRepository,
      IOptions<RedisConfig> redisConfig,
      IGlobalCacheRepository globalCache)
    {
      _officeUserRepository = officeUserRepository;
      _redisConfig = redisConfig;
      _globalCache = globalCache;
    }

    public async Task Consume(ConsumeContext<IGetOfficesRequest> context)
    {
      List<OfficeData> offices = null;

      offices = await GetOfficesAsync(context.Message.UserIds);

      object response = OperationResultWrapper.CreateResponse((_) => IGetOfficesResponse.CreateObj(offices), context);

      await context.RespondAsync<IOperationResult<IGetOfficesResponse>>(response);

      await CreateCache(context.Message.UserIds, offices);
    }
  }
}

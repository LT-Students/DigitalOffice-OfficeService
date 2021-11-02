using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.OfficeService.Data.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Db;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Configurations;
using MassTransit;
using Microsoft.Extensions.Options;
using LT.DigitalOffice.Models.Broker.Requests.Office;
using LT.DigitalOffice.Models.Broker.Models.Office;
using LT.DigitalOffice.Models.Broker.Responses.Office;

namespace LT.DigitalOffice.OfficeService.Broker.Consumers
{
  public class GetOfficesConsumer : IConsumer<IGetOfficesRequest>
  {
    private readonly IOfficeUserRepository _officeUserRepository;
    private readonly IRedisHelper _redisHelper;
    private readonly IOptions<RedisConfig> _redisConfig;
    private readonly ICacheNotebook _cacheNotebook;

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
          o.Latitude,
          o.Longitude,
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
        await _redisHelper.CreateAsync(Cache.Offices, key, offices, TimeSpan.FromMinutes(_redisConfig.Value.CacheLiveInMinutes));

        _cacheNotebook.Add(offices.Select(o => o.Id).ToList(), Cache.Offices, key);
      }
    }

    public GetOfficesConsumer(
      IOfficeUserRepository officeUserRepository,
      IRedisHelper redisHelper,
      IOptions<RedisConfig> redisConfig,
      ICacheNotebook cacheNotebook)
    {
      _officeUserRepository = officeUserRepository;
      _redisHelper = redisHelper;
      _redisConfig = redisConfig;
      _cacheNotebook = cacheNotebook;
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

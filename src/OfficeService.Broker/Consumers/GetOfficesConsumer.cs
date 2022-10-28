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
using LT.DigitalOffice.OfficeService.DataLayer;
using LT.DigitalOffice.OfficeService.DataLayer.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LT.DigitalOffice.OfficeService.Broker.Consumers
{
  public class GetOfficesConsumer : IConsumer<IGetOfficesRequest>
  {
    private readonly OfficeServiceDbContext _dbContext;
    private readonly IOptions<RedisConfig> _redisConfig;
    private readonly IGlobalCacheRepository _globalCache;

    private async Task<List<DbOfficeUser>> GetOfficeUsersAsync(List<Guid> usersIds)
    {
      IQueryable<DbOfficeUser> users = _dbContext.OfficesUsers
        .Where(u => usersIds.Contains(u.UserId) && u.IsActive)
        .Include(ou => ou.Office)
        .AsQueryable();

      return await users.ToListAsync();
    }

    private async Task<List<OfficeData>> GetOfficesAsync(List<Guid> userIds)
    {
      List<DbOffice> offices = (await GetOfficeUsersAsync(userIds))
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

    public GetOfficesConsumer(
      OfficeServiceDbContext dbContext,
      IOptions<RedisConfig> redisConfig,
      IGlobalCacheRepository globalCache)
    {
      _dbContext = dbContext;
      _redisConfig = redisConfig;
      _globalCache = globalCache;
    }

    public async Task Consume(ConsumeContext<IGetOfficesRequest> context)
    {
      List<OfficeData> offices = await GetOfficesAsync(context.Message.UserIds);

      object response = OperationResultWrapper.CreateResponse(_ => IGetOfficesResponse.CreateObj(offices), context);

      await context.RespondAsync<IOperationResult<IGetOfficesResponse>>(response);

      if (offices is not null && offices.Any())
      {
        await _globalCache.CreateAsync(
          Cache.Offices,
          context.Message.UserIds.GetRedisCacheKey(context.Message.GetBasicProperties()),
          offices,
          offices.Select(o => o.Id).ToList(),
          TimeSpan.FromMinutes(_redisConfig.Value.CacheLiveInMinutes));
      }
    }
  }
}

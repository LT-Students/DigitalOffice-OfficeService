using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.OfficeService.Business.Commands.Users.Interfaces;
using LT.DigitalOffice.OfficeService.Data.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Users;
using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.OfficeService.Business.Commands.Users
{
  public class RemoveOfficeUsersCommand : IRemoveOfficeUsersCommand
  {
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAccessValidator _accessValidator;
    private readonly IResponseCreator _responseCreator;
    private readonly IOfficeUserRepository _repository;
    private readonly IGlobalCacheRepository _globalCache;

    public RemoveOfficeUsersCommand(
      IHttpContextAccessor httpContextAccessor,
      IAccessValidator accessValidator,
      IResponseCreator responseCreator,
      IOfficeUserRepository repository,
      IGlobalCacheRepository globalCache)
    {
      _httpContextAccessor = httpContextAccessor;
      _accessValidator = accessValidator;
      _responseCreator = responseCreator;
      _repository = repository;
      _globalCache = globalCache;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(RemoveOfficeUsers request)
    {
      if (request.UsersIds.All(u => u != _httpContextAccessor.HttpContext.GetUserId())
        && !await _accessValidator.HasRightsAsync(Rights.AddEditRemoveUsers))
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.Forbidden);
      }

      List<Guid> removedUsersIds = await _repository.RemoveAsync(request.UsersIds, request.OfficeId);

      if (!removedUsersIds.Any())
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.BadRequest);
      }

      foreach (Guid removedUserId in removedUsersIds)
      {
        await _globalCache.RemoveAsync(removedUserId);
      }
      OperationResultResponse<bool> response = new() { Body = true };

      return response;
    }
  }
}

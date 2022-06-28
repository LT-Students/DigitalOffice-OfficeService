using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentValidation.Results;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.OfficeService.Business.Commands.Users.Interfaces;
using LT.DigitalOffice.OfficeService.Data.Interfaces;
using LT.DigitalOffice.OfficeService.Mappers.Db.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Users;
using LT.DigitalOffice.OfficeService.Validation.Users.Interfaces;
using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.OfficeService.Business.Commands.Users
{
  public class CreateOfficeUsersCommand : ICreateOfficeUsersCommand
  {
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAccessValidator _accessValidator;
    private readonly IResponseCreator _responseCreator;
    private readonly IDbOfficeUserMapper _mapper;
    private readonly ICreateOfficeUsersValidator _validator;
    private readonly IOfficeUserRepository _repository;
    private readonly IGlobalCacheRepository _globalCache;

    public CreateOfficeUsersCommand(
      IHttpContextAccessor httpContextAccessor,
      IAccessValidator accessValidator,
      IResponseCreator responseCreator,
      IDbOfficeUserMapper mapper,
      ICreateOfficeUsersValidator validator,
      IOfficeUserRepository repository,
      IGlobalCacheRepository globalCache)
    {
      _httpContextAccessor = httpContextAccessor;
      _accessValidator = accessValidator;
      _responseCreator = responseCreator;
      _mapper = mapper;
      _validator = validator;
      _repository = repository;
      _globalCache = globalCache;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(CreateOfficeUsers request)
    {
      if (request.UsersIds.All(u => u != _httpContextAccessor.HttpContext.GetUserId())
          && !await _accessValidator.HasRightsAsync(Rights.AddEditRemoveUsers))
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.Forbidden);
      }

      ValidationResult validationResult = await _validator.ValidateAsync(request);
      if (!validationResult.IsValid)
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.BadRequest,
          validationResult.Errors.Select(e => e.ErrorMessage).ToList());
      }

      List<Guid> existingUsersIds = (await _repository.GetAsync(request.UsersIds))
        ?.Select(eu => eu.UserId)
        .ToList();

      List<Guid> removedUsersIds = await _repository.RemoveAsync(existingUsersIds);
      if (removedUsersIds is not null)
      {
        foreach (Guid removedUserId in removedUsersIds)
        {
          await _globalCache.RemoveAsync(removedUserId);
        }
      }

      OperationResultResponse<bool> response = new();

      response.Body = await _repository.CreateAsync(_mapper.Map(request));

      return response;
    }
  }
}

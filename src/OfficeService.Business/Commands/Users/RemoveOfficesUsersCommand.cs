﻿using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentValidation.Results;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.OfficeService.Business.Commands.Users.Interfaces;
using LT.DigitalOffice.OfficeService.Data.Interfaces;
using LT.DigitalOffice.OfficeService.Mappers.Db.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Users;
using LT.DigitalOffice.OfficeService.Validation.Users.Interfaces;

namespace LT.DigitalOffice.OfficeService.Business.Commands.Users
{
  public class RemoveOfficesUsersCommand : IRemoveOfficesUsersCommand
  {
    private readonly IAccessValidator _accessValidator;
    private readonly IResponseCreator _responseCreator;
    private readonly IRemoveOfficesUsersRequestValidator _validator;
    private readonly IOfficeUserRepository _repository;

    public RemoveOfficesUsersCommand(
      IAccessValidator accessValidator,
      IResponseCreator responseCreator,
      IDbOfficeUserMapper mapper,
      IRemoveOfficesUsersRequestValidator validator,
      IOfficeUserRepository repository)
    {
      _accessValidator = accessValidator;
      _responseCreator = responseCreator;
      _validator = validator;
      _repository = repository;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(RemoveOfficesUsersRequest request)
    {
      if (!await _accessValidator.HasRightsAsync(Rights.AddEditRemoveUsers))
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.Forbidden);
      }

      ValidationResult validationResult = await _validator.ValidateAsync(request);
      if (!validationResult.IsValid)
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.BadRequest,
          validationResult.Errors.Select(e => e.ErrorMessage).ToList());
      }

      bool result = await _repository.RemoveAsync(request.UsersIds, request.OfficeId);

      return new()
      {
        Status = result ? OperationResultStatusType.FullSuccess : OperationResultStatusType.Failed,
        Body = result
      };
    }
  }
}

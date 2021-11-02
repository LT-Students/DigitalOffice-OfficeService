using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentValidation.Results;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.OfficeService.Business.Commands.Users.Interfaces;
using LT.DigitalOffice.OfficeService.Data.Interfaces;
using LT.DigitalOffice.OfficeService.Mappers.Db.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Users;
using LT.DigitalOffice.OfficeService.Validation.Users.Interfaces;
using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.OfficeService.Business.Commands.Users
{
  public class ChangeOfficeCommand : IChangeOfficeCommand
  {
    private readonly IAccessValidator _accessValidator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IResponseCreater _responseCreater;
    private readonly IChangeOfficeRequestValidator _validator;
    private readonly IDbOfficeUserMapper _mapper;
    private readonly IOfficeUserRepository _repository;
    private readonly ICacheNotebook _cacheNotebook;

    public ChangeOfficeCommand(
      IAccessValidator accessValidator,
      IHttpContextAccessor httpContextAccessor,
      IResponseCreater responseCreater,
      IChangeOfficeRequestValidator validator,
      IDbOfficeUserMapper mapper,
      IOfficeUserRepository repository,
      ICacheNotebook cacheNotebook)
    {
      _accessValidator = accessValidator;
      _httpContextAccessor = httpContextAccessor;
      _responseCreater = responseCreater;
      _validator = validator;
      _mapper = mapper;
      _repository = repository;
      _cacheNotebook = cacheNotebook;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(ChangeOfficeRequest request)
    {
      if (_httpContextAccessor.HttpContext.GetUserId() != request.UserId
        && !await _accessValidator.HasRightsAsync(Rights.AddEditRemoveUsers))
      {
        return _responseCreater.CreateFailureResponse<bool>(HttpStatusCode.Forbidden);
      }

      ValidationResult validationResult = await _validator.ValidateAsync(request);

      if (!validationResult.IsValid)
      {
        return _responseCreater.CreateFailureResponse<bool>(HttpStatusCode.BadRequest,
          validationResult.Errors.Select(e => e.ErrorMessage).ToList());
      }

      Guid? removedOfficeId = await _repository.RemoveAsync(request.UserId, _httpContextAccessor.HttpContext.GetUserId());

      if (removedOfficeId.HasValue)
      {
        await _cacheNotebook.RemoveAsync(removedOfficeId.Value);
      }

      bool result = !request.OfficeId.HasValue || await _repository.CreateAsync(_mapper.Map(request));

      if (result && request.OfficeId.HasValue)
      {
        await _cacheNotebook.RemoveAsync(request.OfficeId.Value);
      }

      return new()
      {
        Status = result ? OperationResultStatusType.FullSuccess : OperationResultStatusType.Failed,
        Body = result
      };
    }
  }
}

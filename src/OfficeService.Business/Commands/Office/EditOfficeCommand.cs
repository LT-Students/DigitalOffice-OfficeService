using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentValidation.Results;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.OfficeService.Business.Commands.Office.Interface;
using LT.DigitalOffice.OfficeService.Data.Interfaces;
using LT.DigitalOffice.OfficeService.Mappers.Models.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Office;
using LT.DigitalOffice.OfficeService.Validation.Office.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace LT.DigitalOffice.OfficeService.Business.Commands.Office
{
  public class EditOfficeCommand : IEditOfficeCommand
  {
    private readonly IAccessValidator _accessValidator;
    private readonly IOfficeRepository _officeRepository;
    private readonly IOfficeUserRepository _officeUserRepository;
    private readonly IPatchDbOfficeMapper _mapper;
    private readonly IEditOfficeRequestValidator _validator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IGlobalCacheRepository _globalCache;
    private readonly IResponseCreator _responseCreator;

    public EditOfficeCommand(
      IAccessValidator accessValidator,
      IOfficeRepository officeRepository,
      IOfficeUserRepository officeUserRepository,
      IPatchDbOfficeMapper mapper,
      IEditOfficeRequestValidator validator,
      IHttpContextAccessor httpContextAccessor,
      IGlobalCacheRepository globalCache,
      IResponseCreator responseCreator)
    {
      _accessValidator = accessValidator;
      _officeRepository = officeRepository;
      _officeUserRepository = officeUserRepository;
      _mapper = mapper;
      _validator = validator;
      _httpContextAccessor = httpContextAccessor;
      _globalCache = globalCache;
      _responseCreator = responseCreator;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(Guid officeId, JsonPatchDocument<EditOfficeRequest> request)
    {
      if (!await _accessValidator.HasRightsAsync(Rights.AddEditRemoveCompanies))
      {
        _responseCreator.CreateFailureResponse<bool>(
          HttpStatusCode.Forbidden);
      }

      ValidationResult validationResult = await _validator.ValidateAsync(request);

      if (!validationResult.IsValid)
      {
        _responseCreator.CreateFailureResponse<bool>(
          HttpStatusCode.BadRequest,
          validationResult.Errors.Select(validationFailure => validationFailure.ErrorMessage).ToList());
      }

      OperationResultResponse<bool> response = new();

      response.Body = await _officeRepository.EditAsync(officeId, _mapper.Map(request));
      response.Status = OperationResultStatusType.FullSuccess;

      if (!response.Body)
      {
        _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.BadRequest);
      }

      Operation<EditOfficeRequest> isActiveOperation = request.Operations.FirstOrDefault(
        o => o.path.EndsWith(nameof(EditOfficeRequest.IsActive), StringComparison.OrdinalIgnoreCase));

      if (isActiveOperation != default && !bool.Parse(isActiveOperation.value.ToString().Trim()))
      {
        await _officeUserRepository.RemoveAsync(officeId);
      }

      await _globalCache.RemoveAsync(officeId);

      return response;
    }
  }
}

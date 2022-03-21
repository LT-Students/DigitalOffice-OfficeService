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

    public EditOfficeCommand(
      IAccessValidator accessValidator,
      IOfficeRepository officeRepository,
      IOfficeUserRepository officeUserRepository,
      IPatchDbOfficeMapper mapper,
      IEditOfficeRequestValidator validator,
      IHttpContextAccessor httpContextAccessor,
      IGlobalCacheRepository globalCache)
    {
      _accessValidator = accessValidator;
      _officeRepository = officeRepository;
      _officeUserRepository = officeUserRepository;
      _mapper = mapper;
      _validator = validator;
      _httpContextAccessor = httpContextAccessor;
      _globalCache = globalCache;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(Guid officeId, JsonPatchDocument<EditOfficeRequest> request)
    {
      if (!await _accessValidator.HasRightsAsync(Rights.AddEditRemoveCompanies))
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;

        return new OperationResultResponse<bool>
        {
          Status = OperationResultStatusType.Failed,
          Errors = new() { "Not enough rights." }
        };
      }

      ValidationResult validationResult = await _validator.ValidateAsync((officeId, request));

      if (!validationResult.IsValid)
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        return new OperationResultResponse<bool>
        {
          Status = OperationResultStatusType.Failed,
          Errors = validationResult.Errors.Select(validationFailure => validationFailure.ErrorMessage).ToList()
        };
      }

      OperationResultResponse<bool> response = new();

      response.Body = await _officeRepository.EditAsync(officeId, _mapper.Map(request));
      response.Status = OperationResultStatusType.FullSuccess;

      if (!response.Body)
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        response.Status = OperationResultStatusType.Failed;
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

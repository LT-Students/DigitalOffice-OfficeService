using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.OfficeService.Business.Commands.Office.Interface;
using LT.DigitalOffice.OfficeService.Data.Interfaces;
using LT.DigitalOffice.OfficeService.Mappers.Models.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Office;
using LT.DigitalOffice.OfficeService.Validation.Office.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.OfficeService.Business.Commands.Office
{
  public class EditOfficeCommand : IEditOfficeCommand
  {
    private readonly IAccessValidator _accessValidator;
    private readonly IOfficeRepository _officeRepository;
    private readonly IPatchDbOfficeMapper _mapper;
    private readonly IEditOfficeRequestValidator _validator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public EditOfficeCommand(
      IAccessValidator accessValidator,
      IOfficeRepository officeRepository,
      IPatchDbOfficeMapper mapper,
      IEditOfficeRequestValidator validator,
      IHttpContextAccessor httpContextAccessor)
    {
      _accessValidator = accessValidator;
      _officeRepository = officeRepository;
      _mapper = mapper;
      _validator = validator;
      _httpContextAccessor = httpContextAccessor;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(Guid officeId, JsonPatchDocument<EditOfficeRequest> request)
    {
      if (!await _accessValidator.HasRightsAsync(Rights.EditCompany))
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;

        return new OperationResultResponse<bool>
        {
          Status = OperationResultStatusType.Failed,
          Errors = new() { "Not enough rights." }
        };
      }

      if (!_validator.ValidateCustom(request, out List<string> errors))
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        return new OperationResultResponse<bool>
        {
          Status = OperationResultStatusType.Failed,
          Errors = errors
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

      return response;
    }
  }
}

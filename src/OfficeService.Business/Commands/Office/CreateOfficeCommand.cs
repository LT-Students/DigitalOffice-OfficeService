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
using LT.DigitalOffice.OfficeService.Mappers.Db.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Db;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Office;
using LT.DigitalOffice.OfficeService.Validation.Office.Interfaces;
using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.OfficeService.Business.Commands.Office
{
  public class CreateOfficeCommand : ICreateOfficeCommand
  {
    private readonly IAccessValidator _accessValidator;
    private readonly IOfficeRepository _officeRepository;
    private readonly IDbOfficeMapper _mapper;
    private readonly ICreateOfficeRequestValidator _validator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateOfficeCommand(
      IAccessValidator accessValidator,
      IOfficeRepository officeRepository,
      IDbOfficeMapper mapper,
      ICreateOfficeRequestValidator validator,
      IHttpContextAccessor httpContextAccessor)
    {
      _accessValidator = accessValidator;
      _officeRepository = officeRepository;
      _mapper = mapper;
      _validator = validator;
      _httpContextAccessor = httpContextAccessor;
    }

    public async Task<OperationResultResponse<Guid>> ExecuteAsync(CreateOfficeRequest request)
    {
      if (!await _accessValidator.HasRightsAsync(Rights.EditCompany))
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;

        return new OperationResultResponse<Guid>
        {
          Status = OperationResultStatusType.Failed,
          Errors = new() { "Not enough rights." }
        };
      }

      if (!_validator.ValidateCustom(request, out List<string> errors))
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        return new OperationResultResponse<Guid>
        {
          Status = OperationResultStatusType.Failed,
          Errors = errors
        };
      }

      DbOffice office = _mapper.Map(request);
      await _officeRepository.CreateAsync(office);

      _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;

      return new OperationResultResponse<Guid>
      {
        Status = OperationResultStatusType.FullSuccess,
        Body = office.Id
      };
    }
  }
}

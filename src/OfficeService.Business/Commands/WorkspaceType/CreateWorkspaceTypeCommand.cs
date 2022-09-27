using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentValidation.Results;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.OfficeService.Business.Commands.WorkspaceType.Interfaces;
using LT.DigitalOffice.OfficeService.Data.WorkspaceType.Interfaces;
using LT.DigitalOffice.OfficeService.Mappers.Db.WorkspaceType.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.WorkspaceType;
using LT.DigitalOffice.OfficeService.Validation.WorkspaceType.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.OfficeService.Business.Commands.WorkspaceType
{
  public class CreateWorkspaceTypeCommand : ICreateWorkspaceTypeCommand
  {
    private readonly IAccessValidator _accessValidator;
    private readonly IWorkspaceTypeRepository _repository;
    private readonly IDbWorkspaceTypeMapper _mapper;
    private readonly ICreateWorkspaceTypeRequestValidator _requestValidator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IResponseCreator _responseCreator;
    private readonly ILogger<CreateWorkspaceTypeCommand> _logger;

    public CreateWorkspaceTypeCommand(
      IAccessValidator accessValidator,
      IWorkspaceTypeRepository repository,
      IDbWorkspaceTypeMapper mapper,
      ICreateWorkspaceTypeRequestValidator requestValidator,
      IHttpContextAccessor httpContextAccessor,
      IResponseCreator responseCreator,
      ILogger<CreateWorkspaceTypeCommand> logger)
    {
      _accessValidator = accessValidator;
      _repository = repository;
      _mapper = mapper;
      _requestValidator = requestValidator;
      _httpContextAccessor = httpContextAccessor;
      _responseCreator = responseCreator;
      _logger = logger;
    }

    public async Task<OperationResultResponse<Guid?>> ExecuteAsync(CreateWorkspaceTypeRequest request)
    {
      //TODO: REMOVE
      _logger.LogInformation($"DEBUG DATA: {_httpContextAccessor.HttpContext.Request.Host.Host}");
      _logger.LogInformation(_httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString());

      if (!await _accessValidator.HasRightsAsync(
            Rights.AddEditRemoveCompanyData,
            Rights.AddEditRemoveCompanies))
      {
        return _responseCreator.CreateFailureResponse<Guid?>(
          HttpStatusCode.Forbidden);
      }

      ValidationResult validationResult = await _requestValidator.ValidateAsync(request);

      if (!validationResult.IsValid)
      {
        return _responseCreator.CreateFailureResponse<Guid?>(
          HttpStatusCode.BadRequest,
          validationResult.Errors.Select(validationFailure => validationFailure.ErrorMessage).ToList());
      }

      OperationResultResponse<Guid?> response = new();

      response.Body = await _repository.CreateAsync(_mapper.Map(request));

      _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;

      if (response.Body == default)
      {
        response = _responseCreator.CreateFailureResponse<Guid?>(HttpStatusCode.BadRequest);
      }

      return response;
    }
  }
}

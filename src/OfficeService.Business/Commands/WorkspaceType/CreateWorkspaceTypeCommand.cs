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

    public CreateWorkspaceTypeCommand(
      IAccessValidator accessValidator,
      IWorkspaceTypeRepository repository,
      IDbWorkspaceTypeMapper mapper,
      ICreateWorkspaceTypeRequestValidator requestValidator,
      IHttpContextAccessor httpContextAccessor,
      IResponseCreator responseCreator)
    {
      _accessValidator = accessValidator;
      _repository = repository;
      _mapper = mapper;
      _requestValidator = requestValidator;
      _httpContextAccessor = httpContextAccessor;
      _responseCreator = responseCreator;
    }

    public async Task<OperationResultResponse<Guid>> ExecuteAsync(CreateWorkspaceTypeRequest request)
    {
      if (!await _accessValidator.HasRightsAsync(
            Rights.AddEditRemoveCompanyData,
            Rights.AddEditRemoveCompanies))
      {
        _responseCreator.CreateFailureResponse<Guid>(
          HttpStatusCode.Forbidden);
      }

      ValidationResult validationResult = await _requestValidator.ValidateAsync(request);

      if (!validationResult.IsValid)
      {
        _responseCreator.CreateFailureResponse<Guid>(
          HttpStatusCode.BadRequest,
          validationResult.Errors.Select(validationFailure => validationFailure.ErrorMessage).ToList());
      }

      await _repository.CreateAsync(_mapper.Map(request));

      _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;

      return new OperationResultResponse<Guid>
      {
        Body = _mapper.Map(request).Id
      };
    }
  }
}

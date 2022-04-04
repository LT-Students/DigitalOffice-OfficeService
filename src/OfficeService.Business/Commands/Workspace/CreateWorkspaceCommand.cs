using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.OfficeService.Business.Commands.Workspace.Interfaces;
using LT.DigitalOffice.OfficeService.Data.Workspace.Interfaces;
using LT.DigitalOffice.OfficeService.Mappers.Db.Workspace.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Workspace;
using LT.DigitalOffice.OfficeService.Validation.Workspace.Interfaces;
using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.OfficeService.Business.Commands.Workspace
{
  public class CreateWorkspaceCommand : ICreateWorkspaceCommand
  {
    private readonly IAccessValidator _accessValidator;
    private readonly IWorkspaceRepository _repository;
    private readonly IDbWorkspaceMapper _mapper;
    private readonly ICreateWorkspaceRequestValidator _requestValidator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IResponseCreator _responseCreator;

    public CreateWorkspaceCommand(
      IAccessValidator accessValidator,
      IWorkspaceRepository repository,
      IDbWorkspaceMapper mapper,
      ICreateWorkspaceRequestValidator requestValidator,
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
    public async Task<OperationResultResponse<Guid>> ExecuteAsync(CreateWorkspaceRequest request)
    {
      if (!await _accessValidator.HasRightsAsync(Rights.AddEditRemoveCompanyData))
      {
        _responseCreator.CreateFailureResponse<Guid>(
          HttpStatusCode.Forbidden);
      }

      var validationResult = await _requestValidator.ValidateAsync(request);

      if (!validationResult.IsValid)
      {
        _responseCreator.CreateFailureResponse<Guid>(
          HttpStatusCode.BadRequest,
          validationResult.Errors.Select(validationFailure => validationFailure.ErrorMessage).ToList());
      }

      var workspace = _mapper.Map(request);
      await _repository.CreateAsync(workspace);

      _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;

      return new OperationResultResponse<Guid>
      {
        Status = OperationResultStatusType.FullSuccess, 
        Body = workspace.Id
      };
    }
  }
}

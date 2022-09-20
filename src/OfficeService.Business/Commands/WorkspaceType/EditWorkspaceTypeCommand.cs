using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentValidation.Results;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.OfficeService.Business.Commands.WorkspaceType.Interfaces;
using LT.DigitalOffice.OfficeService.Data.WorkspaceType.Interfaces;
using LT.DigitalOffice.OfficeService.Mappers.Models.WorkspaceType.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.WorkspaceType;
using LT.DigitalOffice.OfficeService.Validation.WorkspaceType.Interfaces;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.OfficeService.Business.Commands.WorkspaceType;

public class EditWorkspaceTypeCommand : IEditWorkspaceTypeCommand
{
  private readonly IAccessValidator _accessValidator;
  private readonly IEditWorkspaceTypeRequestValidator _validator;
  private readonly IWorkspaceTypeRepository _workspaceTypeRepository;
  private readonly IResponseCreator _responseCreator;
  private readonly IPatchDbWorkspaceTypeMapper _mapper;
  private readonly IGlobalCacheRepository _globalCache;

  public EditWorkspaceTypeCommand(
    IAccessValidator accessValidator,
    IEditWorkspaceTypeRequestValidator validator,
    IWorkspaceTypeRepository workspaceTypeRepository,
    IResponseCreator responseCreator,
    IPatchDbWorkspaceTypeMapper mapper,
    IGlobalCacheRepository globalCache)
  {
    _accessValidator = accessValidator;
    _validator = validator;
    _workspaceTypeRepository = workspaceTypeRepository;
    _responseCreator = responseCreator;
    _mapper = mapper;
    _globalCache = globalCache;
  }

  public async Task<OperationResultResponse<bool>> Execute(Guid workspaceTypeId, JsonPatchDocument<EditWorkspaceTypeRequest> request)
  {
    if (!await _accessValidator.HasRightsAsync(Rights.AddEditRemoveCompanyData) 
      && !await _accessValidator.HasRightsAsync(Rights.AddEditRemoveCompanies))
    {
      return _responseCreator.CreateFailureResponse<bool>(
        HttpStatusCode.Forbidden);
    }

    ValidationResult validationResult = await _validator.ValidateAsync(request);

    if (!validationResult.IsValid)
    {
      return _responseCreator.CreateFailureResponse<bool>(
        HttpStatusCode.BadRequest,
        validationResult.Errors.Select(validationFailure => validationFailure.ErrorMessage).ToList());
    }

    OperationResultResponse<bool> response = new();

    response.Body = await _workspaceTypeRepository.EditAsync(workspaceTypeId, _mapper.Map(request));

    if (!response.Body)
    {
      _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.BadRequest);
    }

    await _globalCache.RemoveAsync(workspaceTypeId);

    return response;
  }
}

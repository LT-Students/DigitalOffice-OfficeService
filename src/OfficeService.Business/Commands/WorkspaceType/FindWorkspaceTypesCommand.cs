using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Kernel.Validators.Interfaces;
using LT.DigitalOffice.OfficeService.Business.Commands.WorkspaceType.Interfaces;
using LT.DigitalOffice.OfficeService.Data.WorkspaceType.Interfaces;
using LT.DigitalOffice.OfficeService.Mappers.Models.WorkspaceType.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Db;
using LT.DigitalOffice.OfficeService.Models.Dto.Models.Workspace;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.WorkspaceType.Filters;
using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.OfficeService.Business.Commands.WorkspaceType
{
  public class FindWorkspaceTypesCommand : IFindWorkspaceTypesCommand
  {
    private readonly IWorkspaceTypeRepository _workspaceTypeRepository;
    private readonly IWorkspaceTypeInfoMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IBaseFindFilterValidator _baseFindValidator;
    private readonly IResponseCreator _responseCreator;

    public FindWorkspaceTypesCommand(
      IWorkspaceTypeRepository workspaceTypeRepository,
      IWorkspaceTypeInfoMapper mapper,
      IHttpContextAccessor httpContextAccessor,
      IBaseFindFilterValidator baseFindValidator,
      IResponseCreator responseCreator)
    {
      _workspaceTypeRepository = workspaceTypeRepository;
      _mapper = mapper;
      _httpContextAccessor = httpContextAccessor;
      _baseFindValidator = baseFindValidator;
      _responseCreator = responseCreator;
    }

    public async Task<FindResultResponse<WorkspaceTypeInfo>> ExecuteAsync(WorkspaceTypeFindFilter filter)
    {
      if (!_baseFindValidator.ValidateCustom(filter, out List<string> errors))
      {
        return _responseCreator.CreateFailureFindResponse<WorkspaceTypeInfo>(
          HttpStatusCode.BadRequest,
          errors);
      }

      FindResultResponse<WorkspaceTypeInfo> response = new();

      (List<DbWorkspaceType> workspaceTypes, int totalCount) = await _workspaceTypeRepository.FindAsync(filter);

      response.Body = workspaceTypes
        .Select(_mapper.Map)
        .ToList();

      response.TotalCount = totalCount;

      return response;
    }
  }
}

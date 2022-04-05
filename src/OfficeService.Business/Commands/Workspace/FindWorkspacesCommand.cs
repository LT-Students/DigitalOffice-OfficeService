using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Kernel.Validators.Interfaces;
using LT.DigitalOffice.OfficeService.Business.Commands.Workspace.Interfaces;
using LT.DigitalOffice.OfficeService.Data.Workspace.Interfaces;
using LT.DigitalOffice.OfficeService.Mappers.Models.Workspace.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Db;
using LT.DigitalOffice.OfficeService.Models.Dto.Models.Workspace;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Workspace.Filters;

using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.OfficeService.Business.Commands.Workspace
{
  public class FindWorkspacesCommand : IFindWorkspacesCommand
  {
    private readonly IWorkspaceRepository _workspaceRepository;
    private readonly IWorkspaceInfoMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IBaseFindFilterValidator _baseFindValidator;
    private readonly IResponseCreator _responseCreator;

    public FindWorkspacesCommand(
      IWorkspaceRepository workspaceRepository,
      IWorkspaceInfoMapper mapper,
      IHttpContextAccessor httpContextAccessor,
      IBaseFindFilterValidator baseFindValidator,
      IResponseCreator responseCreator)
    {
      _workspaceRepository = workspaceRepository;
      _mapper = mapper;
      _httpContextAccessor = httpContextAccessor;
      _baseFindValidator = baseFindValidator;
      _responseCreator = responseCreator;
    }

    public async Task<FindResultResponse<WorkspaceInfo>> ExecuteAsync(WorkspaceFindFilter filter)
    {
      if (!_baseFindValidator.ValidateCustom(filter, out List<string> errors))
      {
        return _responseCreator.CreateFailureFindResponse<WorkspaceInfo>(
          HttpStatusCode.BadRequest,
          errors);
      }

      FindResultResponse<WorkspaceInfo> response = new();

      (List<DbWorkspace> workspaces, int totalCount) = await _workspaceRepository.FindAsync(filter);

      response.Body = workspaces
        .Select(_mapper.Map)
        .ToList();

      response.TotalCount = totalCount;

      return response;
    }
  }
}

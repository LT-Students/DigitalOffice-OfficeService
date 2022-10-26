using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.OfficeService.DataLayer;
using LT.DigitalOffice.OfficeService.DataLayer.Models;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.OfficeService.Business.Workspace.Create
{
  public class CreateWorkspaceHandler : IRequestHandler<CreateWorkspaceRequest, Guid?>
  {
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDataProvider _provider;

    #region private methods

    private async Task<Guid?> CreateWorkspaceAsync(DbWorkspace workspace)
    {
      if (workspace is null)
      {
        return null;
      }

      _provider.Workspaces.Add(workspace);
      await _provider.SaveAsync();

      return workspace.Id;
    }

    private DbWorkspace Map(CreateWorkspaceRequest request)
    {
      if (request is null)
      {
        return null;
      }

      Regex nameRegex = new(@"^\s+|\s+$|\s+(?=\s)");

      return new DbWorkspace
      {
        Id = Guid.NewGuid(),
        ParentId = request.ParentId,
        Name = !string.IsNullOrWhiteSpace(request.Name) ? nameRegex.Replace(request.Name, "") : null,
        WorkspaceTypeId = request.WorkspaceTypeId,
        Description = request.Description.Trim(),
        IsBookable = request.IsBookable,
        CreatedAtUtc = DateTime.UtcNow,
        CreatedBy = _httpContextAccessor.HttpContext.GetUserId(),
        IsActive = true
      };
    }

    #endregion

    public CreateWorkspaceHandler(
      IHttpContextAccessor httpContextAccessor,
      IDataProvider provider)
    {
      _httpContextAccessor = httpContextAccessor;
      _provider = provider;
    }

    public async Task<Guid?> Handle(CreateWorkspaceRequest request, CancellationToken ct)
    {
      return await CreateWorkspaceAsync(Map(request));
    }
  }
}

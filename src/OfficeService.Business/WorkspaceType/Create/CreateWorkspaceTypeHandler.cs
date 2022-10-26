using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.OfficeService.DataLayer;
using LT.DigitalOffice.OfficeService.DataLayer.Models;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.OfficeService.Business.WorkspaceType.Create
{
  public class CreateWorkspaceTypeHandler : IRequestHandler<CreateWorkspaceTypeRequest, Guid?>
  {
    private readonly IDataProvider _provider;
    private readonly IHttpContextAccessor _httpContextAccessor;

    #region private methods

    private async Task<Guid?> CreateWorkspaceTypeAsync(DbWorkspaceType workspaceType)
    {
      if (workspaceType is null)
      {
        return null;
      }

      _provider.WorkspacesTypes.Add(workspaceType);
      await _provider.SaveAsync();

      return workspaceType.Id;
    }

    private DbWorkspaceType Map(CreateWorkspaceTypeRequest request)
    {
      if (request is null)
      {
        return null;
      }

      Regex nameRegex = new(@"^\s+|\s+$|\s+(?=\s)");

      return new DbWorkspaceType
      {
        Id = Guid.NewGuid(),
        Name = !string.IsNullOrWhiteSpace(request.Name) ? nameRegex.Replace(request.Name, "") : null,
        Description = request.Description.Trim(),
        StartTime = request.StartTime,
        EndTime = request.EndTime,
        BookingRule = (int)request.BookingRule,
        CreatedAtUtc = DateTime.UtcNow,
        CreatedBy = _httpContextAccessor.HttpContext.GetUserId(),
        IsActive = true
      };
    }

    #endregion

    public CreateWorkspaceTypeHandler(
      IDataProvider provider,
      IHttpContextAccessor httpContextAccessor)
    {
      _provider = provider;
      _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Guid?> Handle(CreateWorkspaceTypeRequest request, CancellationToken ct)
    {
      return await CreateWorkspaceTypeAsync(Map(request));
    }
  }
}

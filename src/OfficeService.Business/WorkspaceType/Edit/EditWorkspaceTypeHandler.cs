using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.Results;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.OfficeService.Broker.Requests;
using LT.DigitalOffice.OfficeService.Data.Provider;
using LT.DigitalOffice.OfficeService.DataLayer.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.OfficeService.Business.WorkspaceType.Edit;

public class EditWorkspaceTypeHandler : IRequestHandler<EditWorkspaceTypeRequest, bool>
{
  private readonly IDataProvider _provider;
  private readonly IHttpContextAccessor _httpContextAccessor;
  private readonly IEditWorkspaceTypeValidator _validator;
  private readonly IGlobalCacheRepository _globalCache;

  #region private methods

  private async Task<bool> EditWorkspaceTypeAsync(Guid workspaceId, JsonPatchDocument<DbWorkspaceType> request, CancellationToken ct)
  {
    DbWorkspaceType dbWorkspaceType = await _provider.WorkspacesTypes
      .FirstOrDefaultAsync(x => x.Id == workspaceId, ct);

    if (dbWorkspaceType is null || request is null)
    {
      return false;
    }

    request.ApplyTo(dbWorkspaceType);
    dbWorkspaceType.ModifiedBy = _httpContextAccessor.HttpContext.GetUserId();
    dbWorkspaceType.ModifiedAtUtc = DateTime.UtcNow;
    await _provider.SaveAsync();

    return true;
  }

  public JsonPatchDocument<DbWorkspaceType> Map(JsonPatchDocument<EditWorkspaceTypePatch> patch)
  {
    if (patch is null)
    {
      return null;
    }

    JsonPatchDocument<DbWorkspaceType> patchDbWorkspaceType = new();

    foreach (Operation<EditWorkspaceTypePatch> item in patch.Operations)
    {
      if (item.path.EndsWith(nameof(EditWorkspaceTypePatch.BookingRule), StringComparison.OrdinalIgnoreCase))
      {
        patchDbWorkspaceType.Operations.Add(new Operation<DbWorkspaceType>(item.op, item.path, item.from, (int)Enum.Parse<BookingRule>(item.value?.ToString())));
        continue;
      }

      patchDbWorkspaceType.Operations.Add(new Operation<DbWorkspaceType>(item.op, item.path, item.from, item.value));
    }

    return patchDbWorkspaceType;
  }

  #endregion

  public EditWorkspaceTypeHandler(
    IDataProvider provider,
    IHttpContextAccessor httpContextAccessor,
    IEditWorkspaceTypeValidator validator,
    IGlobalCacheRepository globalCache)
  {
    _provider = provider;
    _httpContextAccessor = httpContextAccessor;
    _validator = validator;
    _globalCache = globalCache;
  }

  public async Task<bool> Handle(EditWorkspaceTypeRequest request, CancellationToken ct)
  {
    ValidationResult validationResult = await _validator.ValidateAsync(request.Patch, ct);
    if (!validationResult.IsValid)
    {
      throw new BadRequestException(validationResult.Errors.Select(e => e.ErrorMessage));
    }

    bool result = await EditWorkspaceTypeAsync(request.WorkspaceTypeId, Map(request.Patch), ct);
    if (result)
    {
      await _globalCache.RemoveAsync(request.WorkspaceTypeId);
    }

    return result;
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.OfficeService.Data.Provider;
using LT.DigitalOffice.OfficeService.Models.Db;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.OfficeService.Business.Commands.Office.Edit
{
  public class EditOfficeHandler : IRequestHandler<EditOfficeRequest, bool>
  {
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IGlobalCacheRepository _globalCache;
    private readonly IDataProvider _provider;

    #region private methods

    private async Task<bool> RemoveOfficeUserAsync(Guid officeId)
    {
      List<DbOfficeUser> dbUsers = await _provider.OfficesUsers.Where(x => x.OfficeId == officeId).ToListAsync();
      DateTime modifiedAtUtc = DateTime.UtcNow;
      Guid senderId = _httpContextAccessor.HttpContext.GetUserId();

      foreach (DbOfficeUser user in dbUsers)
      {
        user.IsActive = false;
        user.ModifiedAtUtc = modifiedAtUtc;
        user.ModifiedBy = senderId;
      }

      await _provider.SaveAsync();

      return true;
    }

    private async Task<bool> EditOfficeAsync(Guid officeId, JsonPatchDocument<DbOffice> request, CancellationToken ct)
    {
      DbOffice dbOffice = await _provider.Offices.FirstOrDefaultAsync(x => x.Id == officeId, ct);

      if (dbOffice == null || request == null)
      {
        return false;
      }

      request.ApplyTo(dbOffice);
      dbOffice.ModifiedBy = _httpContextAccessor.HttpContext.GetUserId();
      dbOffice.ModifiedAtUtc = DateTime.UtcNow;
      await _provider.SaveAsync();

      return true;
    }

    private JsonPatchDocument<DbOffice> Map(JsonPatchDocument<EditOfficePatch> request)
    {
      Regex nameRegex = new(@"^\s+|\s+$|\s+(?=\s)");

      if (request == null)
      {
        return null;
      }

      JsonPatchDocument<DbOffice> patchDbNews = new JsonPatchDocument<DbOffice>();

      foreach (Operation<EditOfficePatch> item in request.Operations)
      {
        if (item.path.EndsWith(nameof(EditOfficePatch.Name), StringComparison.OrdinalIgnoreCase))
        {
          patchDbNews.Operations.Add(new Operation<DbOffice>(
            item.op,
            item.path,
            item.from,
            string.IsNullOrWhiteSpace(item.value?.ToString())
              ? null
              : nameRegex.Replace(item.value.ToString(), "")));
        }
        else
        {
          patchDbNews.Operations.Add(new Operation<DbOffice>(
            item.op,
            item.path,
            item.from,
            item.value?.ToString().Trim() == string.Empty
              ? null
              : item.value?.ToString().Trim()));
        }
      }

      return patchDbNews;
    }

    #endregion

    public EditOfficeHandler(
      IHttpContextAccessor httpContextAccessor,
      IGlobalCacheRepository globalCache,
      IDataProvider provider)
    {
      _httpContextAccessor = httpContextAccessor;
      _globalCache = globalCache;
      _provider = provider;
    }

    public async Task<bool> Handle(EditOfficeRequest request, CancellationToken ct)
    {
      bool result = await EditOfficeAsync(request.OfficeId, Map(request.Patch), ct);

      var isActiveOperation = request.Patch.Operations.FirstOrDefault(
        o => o.path.EndsWith(nameof(EditOfficePatch.IsActive), StringComparison.OrdinalIgnoreCase));

      if (isActiveOperation != default && !bool.Parse(isActiveOperation.value.ToString().Trim()))
      {
        await RemoveOfficeUserAsync(request.OfficeId);
      }

      await _globalCache.RemoveAsync(request.OfficeId);

      return result;
    }
  }
}

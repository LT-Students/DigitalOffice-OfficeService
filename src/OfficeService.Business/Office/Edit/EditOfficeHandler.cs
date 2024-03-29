﻿using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.Results;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.OfficeService.DataLayer;
using LT.DigitalOffice.OfficeService.DataLayer.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.OfficeService.Business.Office.Edit
{
  public class EditOfficeHandler : IRequestHandler<EditOfficeRequest, bool>
  {
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IGlobalCacheRepository _globalCache;
    private readonly OfficeServiceDbContext _dbContext;
    private readonly IEditOfficeValidator _validator;

    #region private methods

    private async Task RemoveOfficeUserAsync(Guid officeId)
    {
      IQueryable<DbOfficeUser> dbUsers = _dbContext.OfficesUsers.Where(x => x.OfficeId == officeId);
      DateTime modifiedAtUtc = DateTime.UtcNow;
      Guid senderId = _httpContextAccessor.HttpContext.GetUserId();

      foreach (DbOfficeUser user in dbUsers)
      {
        user.IsActive = false;
        user.ModifiedAtUtc = modifiedAtUtc;
        user.ModifiedBy = senderId;
      }

      await _dbContext.SaveAsync();
    }

    private async Task<bool> EditOfficeAsync(Guid officeId, JsonPatchDocument<DbOffice> request, CancellationToken ct)
    {
      DbOffice dbOffice = await _dbContext.Offices.FirstOrDefaultAsync(x => x.Id == officeId, ct);

      if (dbOffice == null || request == null)
      {
        return false;
      }

      request.ApplyTo(dbOffice);
      dbOffice.ModifiedBy = _httpContextAccessor.HttpContext.GetUserId();
      dbOffice.ModifiedAtUtc = DateTime.UtcNow;
      await _dbContext.SaveAsync();

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
      OfficeServiceDbContext dbContext,
      IEditOfficeValidator validator)
    {
      _httpContextAccessor = httpContextAccessor;
      _globalCache = globalCache;
      _dbContext = dbContext;
      _validator = validator;
    }

    public async Task<bool> Handle(EditOfficeRequest request, CancellationToken ct)
    {
      ValidationResult validationResult = await _validator.ValidateAsync(request.Patch, ct);
      if (!validationResult.IsValid)
      {
        throw new BadRequestException(validationResult.Errors.Select(e => e.ErrorMessage));
      }

      bool result = await EditOfficeAsync(request.OfficeId, Map(request.Patch), ct);

      Operation<EditOfficePatch> isActiveOperation = request.Patch.Operations.FirstOrDefault(
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

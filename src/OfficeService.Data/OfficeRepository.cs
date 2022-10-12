﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.OfficeService.Data.Interfaces;
using LT.DigitalOffice.OfficeService.Data.Provider;
using LT.DigitalOffice.OfficeService.Models.Db;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.OfficeService.Data
{
  public class OfficeRepository : IOfficeRepository
  {
    private readonly IDataProvider _provider;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public OfficeRepository(
      IDataProvider provider,
      IHttpContextAccessor httpContextAccessor)
    {
      _httpContextAccessor = httpContextAccessor;
      _provider = provider;
    }

    public async Task<bool> DoesExistAsync(Guid officeId)
    {
      return await _provider.Offices.AnyAsync(o => o.Id == officeId);
    }

    public async Task<bool> EditAsync(Guid officeId, JsonPatchDocument<DbOffice> request)
    {
      DbOffice dbOffice = await _provider.Offices.FirstOrDefaultAsync(x => x.Id == officeId);

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

    public async Task<DbOffice> GetAsync(Guid officeId)
    {
      return await _provider.Offices.FirstOrDefaultAsync(x => x.Id == officeId);
    }

    public async Task<List<DbOffice>> GetAsync(List<Guid> officesIds)
    {
      return await _provider.Offices.Where(o => officesIds.Contains(o.Id))
        .Include(o => o.Users).Where(u => u.IsActive)
        .ToListAsync();
    }

    public async Task<bool> DoesNameExistAsync(string name)
    {
      return !await _provider.Offices.AnyAsync(x => string.Equals(x.Name.ToLower(), name.ToLower()));
    }
  }
}

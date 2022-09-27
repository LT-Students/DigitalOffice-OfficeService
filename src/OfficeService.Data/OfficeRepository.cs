using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.OfficeService.Data.Interfaces;
using LT.DigitalOffice.OfficeService.Data.Provider;
using LT.DigitalOffice.OfficeService.Models.Db;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Office.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.OfficeService.Data
{
  public class OfficeRepository : IOfficeRepository
  {
    private readonly IDataProvider _provider;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<OfficeRepository> _logger;

    public OfficeRepository(
      IDataProvider provider,
      IHttpContextAccessor httpContextAccessor,
      ILogger<OfficeRepository> logger)
    {
      _httpContextAccessor = httpContextAccessor;
      _provider = provider;
      _logger = logger;
    }

    public async Task CreateAsync(DbOffice office)
    {
      if (office == null)
      {
        _logger.LogWarning(new ArgumentNullException(nameof(office)).Message);
        return;
      }

      _provider.Offices.Add(office);
      await _provider.SaveAsync();
    }

    public async Task<bool> DoesExistAsync(Guid officeId)
    {
      return await _provider.Offices.AnyAsync(o => o.Id == officeId);
    }

    public async Task<(List<DbOffice>, int totalCount)> FindAsync(OfficeFindFilter filter)
    {
      if (filter == null)
      {
        return (null, 0);
      }

      IQueryable<DbOffice> dbOffices = _provider.Offices
        .AsQueryable();

      if (filter.IsActive.HasValue)
      {
        dbOffices = dbOffices.Where(x => x.IsActive == filter.IsActive);
      }

      if (!string.IsNullOrWhiteSpace(filter.NameIncludeSubstring))
      {
        dbOffices = dbOffices.Where(x =>
          x.Name.ToLower().Contains(filter.NameIncludeSubstring.ToLower()));
      }

      if (filter.IsAscendingSort.HasValue)
      {
        dbOffices = filter.IsAscendingSort.Value 
        ? dbOffices.OrderBy(o => o.Name)
        : dbOffices.OrderByDescending(o => o.Name);
      }

      return (await dbOffices.Skip(filter.SkipCount).Take(filter.TakeCount).ToListAsync(), await dbOffices.CountAsync());
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

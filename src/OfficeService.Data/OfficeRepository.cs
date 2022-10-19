using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.OfficeService.Data.Interfaces;
using LT.DigitalOffice.OfficeService.Data.Provider;
using LT.DigitalOffice.OfficeService.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.OfficeService.Data
{
  public class OfficeRepository : IOfficeRepository
  {
    private readonly IDataProvider _provider;

    public OfficeRepository(IDataProvider provider)
    {
      _provider = provider;
    }

    public async Task<bool> DoesExistAsync(Guid officeId)
    {
      return await _provider.Offices.AnyAsync(o => o.Id == officeId);
    }

    public async Task<List<DbOffice>> GetAsync(List<Guid> officesIds)
    {
      return await _provider.Offices.Where(o => officesIds.Contains(o.Id))
        .Include(o => o.Users).Where(u => u.IsActive)
        .ToListAsync();
    }
  }
}

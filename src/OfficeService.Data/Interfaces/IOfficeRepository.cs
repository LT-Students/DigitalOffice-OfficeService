using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.OfficeService.Models.Db;
using LT.DigitalOffice.Kernel.Attributes;
using Microsoft.AspNetCore.JsonPatch;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Office.Filters;

namespace LT.DigitalOffice.OfficeService.Data.Interfaces
{
  [AutoInject]
  public interface IOfficeRepository
  {
    Task CreateAsync(DbOffice office);

    Task<DbOffice> GetAsync(Guid officeId);

    Task<(List<DbOffice>, int totalCount)> FindAsync(OfficeFindFilter filter);

    Task<bool> EditAsync(Guid officeId, JsonPatchDocument<DbOffice> request);

    Task<bool> DoesExistAsync(Guid officeId);
  }
}

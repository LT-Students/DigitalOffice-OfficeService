using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.OfficeService.Models.Db;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.OfficeService.Data.Interfaces
{
  [AutoInject]
  public interface IOfficeRepository
  {
    Task<DbOffice> GetAsync(Guid officeId);

    Task<List<DbOffice>> GetAsync(List<Guid> officesIds);

    Task<bool> EditAsync(Guid officeId, JsonPatchDocument<DbOffice> request);

    Task<bool> DoesExistAsync(Guid officeId);

    Task<bool> DoesNameExistAsync(string name);
  }
}

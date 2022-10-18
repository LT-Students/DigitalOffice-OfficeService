using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.OfficeService.Models.Db;

namespace LT.DigitalOffice.OfficeService.Data.Interfaces
{
  [AutoInject]
  public interface IOfficeRepository
  {
    Task<List<DbOffice>> GetAsync(List<Guid> officesIds);

    Task<bool> DoesNameExistAsync(string name);
  }
}

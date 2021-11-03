using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.OfficeService.Models.Db;
using LT.DigitalOffice.OfficeService.Models.Dto.Models;

namespace LT.DigitalOffice.OfficeService.Mappers.Models.Interfaces
{
  [AutoInject]
  public interface IOfficeInfoMapper
  {
    OfficeInfo Map(DbOffice office);
  }
}

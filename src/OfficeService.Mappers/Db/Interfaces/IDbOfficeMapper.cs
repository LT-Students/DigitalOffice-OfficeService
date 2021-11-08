using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.OfficeService.Models.Db;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Office;

namespace LT.DigitalOffice.OfficeService.Mappers.Db.Interfaces
{
  [AutoInject]
  public interface IDbOfficeMapper
  {
    DbOffice Map(CreateOfficeRequest request);
  }
}

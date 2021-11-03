using LT.DigitalOffice.OfficeService.Models.Db;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Requests.Office;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Users;

namespace LT.DigitalOffice.OfficeService.Mappers.Db.Interfaces
{
  [AutoInject]
  public interface IDbOfficeUserMapper
  {
    DbOfficeUser Map(ICreateUserOfficeRequest request);

    DbOfficeUser Map(ChangeUserOfficeRequest request);
  }
}

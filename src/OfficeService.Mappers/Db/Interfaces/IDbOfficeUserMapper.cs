using System.Collections.Generic;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Publishing.Subscriber.Office;
using LT.DigitalOffice.OfficeService.Models.Db;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Users;

namespace LT.DigitalOffice.OfficeService.Mappers.Db.Interfaces
{
  [AutoInject]
  public interface IDbOfficeUserMapper
  {
    List<DbOfficeUser> Map(CreateOfficeUsers request);
  }
}

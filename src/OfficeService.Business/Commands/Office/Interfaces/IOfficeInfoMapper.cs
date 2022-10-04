using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.OfficeService.Models.Db;

namespace LT.DigitalOffice.OfficeService.Business.Commands.Office.Interfaces
{
  [AutoInject]
  public interface IOfficeInfoMapper
  {
    OfficeInfo Map(DbOffice office);
  }
}

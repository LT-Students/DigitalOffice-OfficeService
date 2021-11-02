using LT.DigitalOffice.OfficeService.Mappers.Models.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Db;
using LT.DigitalOffice.OfficeService.Models.Dto.Models;

namespace LT.DigitalOffice.OfficeService.Mappers.Models
{
  public class OfficeInfoMapper : IOfficeInfoMapper
  {
    public OfficeInfo Map(DbOffice office)
    {
      if (office == null)
      {
        return null;
      }

      return new OfficeInfo
      {
        Id = office.Id,
        Name = office.Name,
        City = office.City,
        Address = office.Address,
        IsActive = office.IsActive
      };
    }
  }
}

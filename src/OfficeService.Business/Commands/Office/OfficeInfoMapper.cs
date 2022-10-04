using LT.DigitalOffice.OfficeService.Business.Commands.Office.Interfaces;
using LT.DigitalOffice.OfficeService.Mappers.Models.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Db;
using LT.DigitalOffice.OfficeService.Models.Dto.Models;

namespace LT.DigitalOffice.OfficeService.Business.Commands.Office
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
        Longitude = office.Longitude,
        Latitude = office.Latitude,
        IsActive = office.IsActive
      };
    }
  }
}

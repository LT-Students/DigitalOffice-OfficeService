using LT.DigitalOffice.OfficeService.Models.Db;
using LT.DigitalOffice.OfficeService.Models.Dto.Models;
using LT.DigitalOffice.Kernel.Attributes;

namespace LT.DigitalOffice.OfficeService.Mappers.Models.Interfaces
{
    [AutoInject]
    public interface IOfficeInfoMapper
    {
        OfficeInfo Map(DbOffice office);
    }
}

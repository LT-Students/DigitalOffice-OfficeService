using LT.DigitalOffice.OfficeService.Models.Db;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Office;
using LT.DigitalOffice.Kernel.Attributes;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.OfficeService.Mappers.Models.Interfaces
{
  [AutoInject]
  public interface IPatchDbOfficeMapper
  {
    JsonPatchDocument<DbOffice> Map(JsonPatchDocument<EditOfficeRequest> request);
  }
}

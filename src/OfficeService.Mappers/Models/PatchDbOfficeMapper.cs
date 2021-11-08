using LT.DigitalOffice.OfficeService.Mappers.Models.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Db;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Office;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace LT.DigitalOffice.OfficeService.Mappers.Models
{
  public class PatchDbOfficeMapper : IPatchDbOfficeMapper
  {
    public JsonPatchDocument<DbOffice> Map(JsonPatchDocument<EditOfficeRequest> request)
    {
      if (request == null)
      {
        return null;
      }

      JsonPatchDocument<DbOffice> patchDbNews = new JsonPatchDocument<DbOffice>();

      foreach (Operation<EditOfficeRequest> item in request.Operations)
      {
        patchDbNews.Operations.Add(new Operation<DbOffice>(
          item.op,
          item.path,
          item.from,
          item.value?.ToString().Trim() == string.Empty ? null : item.value?.ToString().Trim()));
      }

      return patchDbNews;
    }
  }
}

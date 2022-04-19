using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LT.DigitalOffice.OfficeService.Models.Dto.Enums.Workspace
{
  [JsonConverter(typeof(StringEnumConverter))]
  public enum BookingRule
  {
    BookingForbidden,
    PartialBooking,
    FullBooking
  }
}

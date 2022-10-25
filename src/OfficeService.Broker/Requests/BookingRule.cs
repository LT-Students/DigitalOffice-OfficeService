using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LT.DigitalOffice.OfficeService.Broker.Requests
{
  [JsonConverter(typeof(StringEnumConverter))]
  public enum BookingRule
  {
    BookingForbidden,
    PartialBooking,
    FullBooking
  }
}

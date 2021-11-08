using LT.DigitalOffice.Kernel.Requests;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.OfficeService.Models.Dto.Requests.Office.Filters
{
  public record OfficeFindFilter : BaseFindFilter
  {
    [FromQuery(Name = "includeDeactivated")]
    public bool IncludeDeactivated { get; set; } = false;
  }
}

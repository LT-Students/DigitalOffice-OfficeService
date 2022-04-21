using LT.DigitalOffice.Kernel.Requests;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.OfficeService.Models.Dto.Requests.Office.Filters
{
  public record OfficeFindFilter : BaseFindFilter
  {
    [FromQuery(Name = "ascendingsort")]
    public bool? IsAscendingSort { get; set; }

    [FromQuery(Name = "active")]
    public bool? IsActive { get; set; }
  }
}

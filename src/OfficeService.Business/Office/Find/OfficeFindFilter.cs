using DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Kernel.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.OfficeService.Business.Office.Find
{
  public record OfficeFindFilter : BaseFindFilter, IRequest<FindResult<OfficeInfo>>
  {
    [FromQuery(Name = "isAscendingSort")]
    public bool? IsAscendingSort { get; set; }

    [FromQuery(Name = "isActive")]
    public bool? IsActive { get; set; }

    [FromQuery(Name = "nameIncludeSubstring")]
    public string NameIncludeSubstring { get; set; }
  }
}

using LT.DigitalOffice.Kernel.Requests;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.OfficeService.Models.Dto.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.OfficeService.Models.Dto.Requests.Office.Filters
{
  public record OfficeFindFilter : BaseFindFilter, IRequest<FindResultResponse<OfficeInfo>>
  {
    [FromQuery(Name = "isAscendingSort")]
    public bool? IsAscendingSort { get; set; }

    [FromQuery(Name = "isActive")]
    public bool? IsActive { get; set; }

    [FromQuery(Name = "nameIncludeSubstring")]
    public string NameIncludeSubstring { get; set; }
  }
}

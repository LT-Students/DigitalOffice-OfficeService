using LT.DigitalOffice.Kernel.Requests;

using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.OfficeService.Models.Dto.Requests.WorkspaceType.Filters
{
  public record WorkspaceTypeFindFilter : BaseFindFilter
  {
    [FromQuery(Name = "IncludeDeactivated")]
    public bool IncludeDeactivated { get; set; } = false;
  }
}

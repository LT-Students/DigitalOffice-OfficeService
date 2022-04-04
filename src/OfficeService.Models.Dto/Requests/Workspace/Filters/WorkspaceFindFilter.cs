using LT.DigitalOffice.Kernel.Requests;

using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.OfficeService.Models.Dto.Requests.Workspace.Filters
{
  public record WorkspaceFindFilter : BaseFindFilter
  {
    [FromQuery(Name = "IncludeDeactivated")]
    public bool IncludeDeactivated { get; set; } = false;
  }
}

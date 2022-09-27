using System;
using LT.DigitalOffice.OfficeService.Models.Dto.Enums.Workspace;

namespace LT.DigitalOffice.OfficeService.Models.Dto.Requests.WorkspaceType;

public record EditWorkspaceTypeRequest
{
  public string Name { get; set; }

  public string Description { get; set; }

  public DateTime? StartTime { get; set; }

  public DateTime? EndTime { get; set; }

  public BookingRule BookingRule { get; set; }

  public bool IsActive { get; set; }
}

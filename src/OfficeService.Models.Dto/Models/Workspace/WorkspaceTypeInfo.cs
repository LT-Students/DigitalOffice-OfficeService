using System;

namespace LT.DigitalOffice.OfficeService.Models.Dto.Models.Workspace
{
  public record WorkspaceTypeInfo
  {
    public string Name { get; set; }

    public string Description { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public int BookingRule { get; set; }

    public bool IsActive { get; set; }
  }
}

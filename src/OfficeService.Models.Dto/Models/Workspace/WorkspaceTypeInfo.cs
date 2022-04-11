using System;
using LT.DigitalOffice.OfficeService.Models.Dto.Enums.Workspace;

namespace LT.DigitalOffice.OfficeService.Models.Dto.Models.Workspace
{
  public record WorkspaceTypeInfo
  {
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public BookingRule BookingRule { get; set; }

    public bool IsActive { get; set; }
  }
}

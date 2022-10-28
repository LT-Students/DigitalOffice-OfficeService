using System;
using LT.DigitalOffice.OfficeService.Broker.Requests;

namespace LT.DigitalOffice.OfficeService.Business.Workspace.Find
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

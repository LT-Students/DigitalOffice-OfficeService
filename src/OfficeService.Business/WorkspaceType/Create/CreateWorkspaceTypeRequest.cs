using System;
using LT.DigitalOffice.OfficeService.Broker.Requests;
using MediatR;

namespace LT.DigitalOffice.OfficeService.Business.WorkspaceType.Create
{
  public record CreateWorkspaceTypeRequest : IRequest<Guid?>
  {
    public string Name { get; set; }

    public string Description { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public BookingRule BookingRule { get; set; }
  }
}

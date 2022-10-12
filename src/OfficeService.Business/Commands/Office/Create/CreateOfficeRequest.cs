using System;
using MediatR;

namespace LT.DigitalOffice.OfficeService.Business.Commands.Office.Create
{
  public record CreateOfficeRequest : IRequest<Guid?>
  {
    public string Name { get; set; }
    public string City { get; set; }
    public string Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
  }
}

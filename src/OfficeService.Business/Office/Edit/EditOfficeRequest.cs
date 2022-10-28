using System;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.OfficeService.Business.Office.Edit
{
  public record EditOfficeRequest : IRequest<bool>
  {
    public Guid OfficeId { get; set; }
    public JsonPatchDocument<EditOfficePatch> Patch { get; set; }
  }
}

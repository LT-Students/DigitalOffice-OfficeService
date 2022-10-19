using FluentValidation;
using LT.DigitalOffice.Kernel.Attributes;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.OfficeService.Business.Commands.Office.Edit
{
  [AutoInject]
  public interface IEditOfficeValidator : IValidator<JsonPatchDocument<EditOfficePatch>>
  {
  }
}

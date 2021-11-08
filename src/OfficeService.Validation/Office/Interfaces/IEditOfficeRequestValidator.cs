using FluentValidation;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Office;
using LT.DigitalOffice.Kernel.Attributes;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.OfficeService.Validation.Office.Interfaces
{
  [AutoInject]
  public interface IEditOfficeRequestValidator : IValidator<JsonPatchDocument<EditOfficeRequest>>
  {
  }
}

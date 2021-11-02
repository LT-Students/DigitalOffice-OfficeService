using FluentValidation;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Users;

namespace LT.DigitalOffice.OfficeService.Validation.Users.Interfaces
{
  [AutoInject]
  public interface IChangeOfficeRequestValidator : IValidator<ChangeOfficeRequest>
  {
  }
}

using FluentValidation;
using LT.DigitalOffice.OfficeService.Data.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Users;
using LT.DigitalOffice.OfficeService.Validation.Users.Interfaces;

namespace LT.DigitalOffice.OfficeService.Validation.Users
{
  public class RemoveOfficeUsersValidator : AbstractValidator<RemoveOfficeUsers>, IRemoveOfficeUsersValidator
  {
    public RemoveOfficeUsersValidator(
      IOfficeRepository officeRepository)
    {
      RuleFor(r => r.OfficeId)
        .MustAsync(async (id, _) => await officeRepository.DoesExistAsync(id))
        .WithMessage("Office must exist.");
    }
  }
}

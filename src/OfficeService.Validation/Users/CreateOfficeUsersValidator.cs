using System.Collections.Generic;
using FluentValidation;
using LT.DigitalOffice.OfficeService.Broker.Requests.Interfaces;
using LT.DigitalOffice.OfficeService.Data.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Users;
using LT.DigitalOffice.OfficeService.Validation.Users.Interfaces;

namespace LT.DigitalOffice.OfficeService.Validation.Users
{
  public class CreateOfficeUsersValidator : AbstractValidator<CreateOfficeUsers>, ICreateOfficeUsersValidator
  {
    public CreateOfficeUsersValidator(
      IOfficeRepository officeRepository,
      IUserService userService)
    {
      RuleFor(r => r.OfficeId)
        .MustAsync(async (id, _) => await officeRepository.DoesExistAsync(id))
        .WithMessage("Office must exist.");

      RuleFor(r => r.UsersIds)
        .MustAsync(async (usersIds, _) => 
          (await userService.CheckUsersExistence(usersIds, new List<string>())).Count == 1)
        .WithMessage("Users must exist");
    }
  }
}

using System.Collections.Generic;
using FluentValidation;
using LT.DigitalOffice.OfficeService.Broker.Requests.Interfaces;
using LT.DigitalOffice.OfficeService.Data.Interfaces;

namespace LT.DigitalOffice.OfficeService.Business.Commands.Users.Create
{
  public class CreateOfficeUsersValidator : AbstractValidator<CreateOfficeUsersRequest>
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
          (await userService.CheckUsersExistence(usersIds, new List<string>())).Count == usersIds.Count)
        .WithMessage("Users must exist");
    }
  }
}

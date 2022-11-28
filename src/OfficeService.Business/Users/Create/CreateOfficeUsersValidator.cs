using System.Collections.Generic;
using FluentValidation;
using LT.DigitalOffice.OfficeService.Broker.Requests.Interfaces;
using LT.DigitalOffice.OfficeService.DataLayer;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.OfficeService.Business.Users.Create
{
  public class CreateOfficeUsersValidator : AbstractValidator<CreateOfficeUsersRequest>
  {
    public CreateOfficeUsersValidator(
      OfficeServiceDbContext dbContext,
      IUserService userService)
    {
      RuleFor(r => r.OfficeId)
        .MustAsync(async (id, _) => await dbContext.Offices.AnyAsync(o => o.Id == id))
        .WithMessage("Office must exist.");

      RuleFor(r => r.UsersIds)
        .MustAsync(async (usersIds, _) =>
          (await userService.CheckUsersExistence(usersIds, new List<string>())).Count == usersIds.Count)
        .WithMessage("Users must exist");
    }
  }
}

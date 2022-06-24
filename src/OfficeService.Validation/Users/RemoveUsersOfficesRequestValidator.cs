using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using LT.DigitalOffice.OfficeService.Data.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Db;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Users;
using LT.DigitalOffice.OfficeService.Validation.Users.Interfaces;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.OfficeService.Validation.Users
{
  public class RemoveUsersOfficesRequestValidator : AbstractValidator<RemoveOfficesUsersRequest>, IRemoveOfficesUsersRequestValidator
  {
    private readonly ILogger<IRemoveOfficesUsersRequestValidator> _logger;
    private readonly IOfficeUserRepository _officeUserRepository;

    private async Task<bool> CheckOfficesUsersExistence(List<Guid> usersIds, Guid officeId)
    {
      string logMessage = "Cannot check existence of users with id {usersIds} in office with id {officeId}.";

      try
      {
        List<DbOfficeUser> officesUsers = await _officeUserRepository.GetAsync(usersIds, officeId);

        if (officesUsers.Count == usersIds.Count)
        {
          return true;
        }

        _logger.LogWarning(logMessage, usersIds);
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, logMessage, usersIds, officeId);
      }

      return false;
    }

    public RemoveUsersOfficesRequestValidator(
      ILogger<IRemoveOfficesUsersRequestValidator> logger,
      IOfficeUserRepository officeUserRepository,
      IOfficeRepository officeRepository)
    {
      _logger = logger;
      _officeUserRepository = officeUserRepository;

      RuleFor(r => r.OfficeId)
        .MustAsync(async (id, _) => await officeRepository.DoesExistAsync(id))
        .WithMessage("Office must exist.");

      RuleFor(r => r)
        .MustAsync(async (r , _) => await CheckOfficesUsersExistence(r.UsersIds, r.OfficeId))
        .WithMessage("All users must exist in specified office");
    }
  }
}

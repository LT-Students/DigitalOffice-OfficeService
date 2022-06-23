using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
using LT.DigitalOffice.Models.Broker.Common;
using LT.DigitalOffice.OfficeService.Data.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Users;
using LT.DigitalOffice.OfficeService.Validation.Office;
using LT.DigitalOffice.OfficeService.Validation.Users.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.OfficeService.Validation.Users
{
  public class CreateOfficesUsersRequestValidator : AbstractValidator<CreateOfficesUsersRequest>, ICreateOfficesUsersRequestValidator
  {
    private readonly IRequestClient<ICheckUsersExistence> _rcCheckUsers;
    private readonly ILogger<CreateOfficeRequestValidator> _logger;

    private async Task<bool> CheckUsersExistence(List<Guid> usersIds)
    {
      string logMessage = "Cannot check existence of users with id {usersIds}.";

      try
      {
        Response<IOperationResult<ICheckUsersExistence>> response =
          await _rcCheckUsers.GetResponse<IOperationResult<ICheckUsersExistence>>(
            ICheckUsersExistence.CreateObj(usersIds));

        var firstNotSecond = response.Message.Body.UserIds.Except(usersIds).ToList();
        var secondNotFirst = usersIds.Except(response.Message.Body.UserIds).ToList();

        if (!firstNotSecond.Any() && !secondNotFirst.Any())
        {
          return true;
        }

        _logger.LogWarning(logMessage, usersIds);
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, logMessage, usersIds);
      }

      return false;
    }

    public CreateOfficesUsersRequestValidator(
      IRequestClient<ICheckUsersExistence> rcCheckUsers,
      ILogger<CreateOfficeRequestValidator> logger,
      IOfficeRepository officeRepository)
    {
      _rcCheckUsers = rcCheckUsers;
      _logger = logger;

      RuleFor(r => r.OfficeId)
        .MustAsync(async (id, _) => await officeRepository.DoesExistAsync(id))
        .WithMessage("Office must exist.");

      RuleFor(r => r.UsersIds)
        .MustAsync(async (ids, _) => await CheckUsersExistence(ids))
        .WithMessage("Users must exist");
    }
  }
}

using System;
using System.Threading.Tasks;
using FluentValidation;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Models.Broker.Common;
using LT.DigitalOffice.OfficeService.Data.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Users;
using LT.DigitalOffice.OfficeService.Validation.Users.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.OfficeService.Validation.Users
{
  public class ChangeOfficeRequestValidator : AbstractValidator<ChangeUserOfficeRequest>, IChangeOfficeRequestValidator
  {
    private readonly IRequestClient<ICheckUsersExistence> _rcCheckUsers;
    private readonly ILogger<ChangeOfficeRequestValidator> _logger;

    private async Task<bool> CheckUserExistance(Guid userId)
    {
      string logMessage = "Cannot check existance user with id {userId}.";

      try
      {
        Response<IOperationResult<ICheckUsersExistence>> response =
          await _rcCheckUsers.GetResponse<IOperationResult<ICheckUsersExistence>>(
            ICheckUsersExistence.CreateObj(new() { userId}));

        if (response.Message.IsSuccess && response.Message.Body.UserIds.Contains(userId))
        {
          return true;
        }

        _logger.LogWarning(logMessage, userId);
      }
      catch(Exception exc)
      {
        _logger.LogError(exc, logMessage, userId);
      }

      return false;
    }

    public ChangeOfficeRequestValidator(
      IRequestClient<ICheckUsersExistence> rcCheckUsers,
      ILogger<ChangeOfficeRequestValidator> logger,
      IOfficeRepository officeRepository)
    {
      _rcCheckUsers = rcCheckUsers;
      _logger = logger;

      RuleFor(r => r.OfficeId)
        .MustAsync(async (id, _) => !id.HasValue || await officeRepository.DoesExistAsync(id.Value))
        .WithMessage("Office must exist.");

      RuleFor(r => r.UserId)
        .MustAsync(async (id, _) => await CheckUserExistance(id))
        .WithMessage("User must exist.");
    }
  }
}

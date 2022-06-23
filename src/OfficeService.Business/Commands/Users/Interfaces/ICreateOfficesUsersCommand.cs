﻿using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Users;

namespace LT.DigitalOffice.OfficeService.Business.Commands.Users.Interfaces
{
  public interface ICreateOfficesUsersCommand
  {
    Task<OperationResultResponse<bool>> ExecuteAsync(CreateOfficesUsersRequest request);
  }
}

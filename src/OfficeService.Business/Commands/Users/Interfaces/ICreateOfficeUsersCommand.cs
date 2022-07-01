using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Users;

namespace LT.DigitalOffice.OfficeService.Business.Commands.Users.Interfaces
{
  [AutoInject]
  public interface ICreateOfficeUsersCommand
  {
    Task<OperationResultResponse<bool>> ExecuteAsync(CreateOfficeUsers request);
  }
}

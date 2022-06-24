using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Users;

namespace LT.DigitalOffice.OfficeService.Business.Commands.Users.Interfaces
{
  [AutoInject]
  public interface ICreateOfficesUsersCommand
  {
    Task<OperationResultResponse<bool>> ExecuteAsync(CreateOfficesUsersRequest request);
  }
}

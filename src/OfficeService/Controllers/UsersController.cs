using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.OfficeService.Business.Commands.Users.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Users;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.OfficeService.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class UsersController : ControllerBase
  {
    [HttpPost("create")]
    public async Task<OperationResultResponse<bool>> AddUsersAsync(
      [FromServices] ICreateOfficesUsersCommand command,
      [FromBody] CreateOfficesUsersRequest request)
    {
      return await command.ExecuteAsync(request);
    }

    [HttpPut("remove")]
    public async Task<OperationResultResponse<bool>> RemoveUsers(
      [FromServices] IRemoveOfficesUsersCommand command,
      [FromBody] RemoveOfficesUsersRequest request)
    {
      return await command.ExecuteAsync(request);
    }
  }
}

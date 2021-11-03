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
    [HttpPost("change")]
    public async Task<OperationResultResponse<bool>> ChangeOfficeAsync(
      [FromServices] IChangeOfficeCommand command,
      [FromBody] ChangeUserOfficeRequest request)
    {
      return await command.ExecuteAsync(request);
    }
  }
}

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.OfficeService.Business.Commands.Users.Create;
using LT.DigitalOffice.OfficeService.Business.Commands.Users.Remove;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.OfficeService.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class UsersController : ControllerBase
  {
    private readonly IMediator _mediator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAccessValidator _accessValidator;

    public UsersController(
      IMediator mediator,
      IHttpContextAccessor httpContextAccessor,
      IAccessValidator accessValidator)
    {
      _mediator = mediator;
      _httpContextAccessor = httpContextAccessor;
      _accessValidator = accessValidator;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateAsync(
      [FromBody] CreateOfficeUsersRequest request,
      CancellationToken ct)
    {
      if (request.UsersIds.All(u => u != _httpContextAccessor.HttpContext.GetUserId())
          && !await _accessValidator.HasRightsAsync(Rights.AddEditRemoveUsers))
      {
        return StatusCode(403);
      }

      bool result = await _mediator.Send(request, ct);

      return result
        ? Created("/officesUsers", true)
        : BadRequest();
    }

    [HttpDelete("remove")]
    public async Task<IActionResult> RemoveAsync(
      [FromBody] RemoveOfficeUsersRequest request,
      CancellationToken ct)
    {
      if (request.UsersIds.All(u => u != _httpContextAccessor.HttpContext.GetUserId())
          && !await _accessValidator.HasRightsAsync(Rights.AddEditRemoveUsers))
      {
        return StatusCode(403);
      }

      bool result = await _mediator.Send(request, ct);
      return result
        ? Ok(true)
        : BadRequest();
    }
  }
}

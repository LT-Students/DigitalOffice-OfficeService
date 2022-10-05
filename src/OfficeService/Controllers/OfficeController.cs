using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Kernel.Validators.Interfaces;
using LT.DigitalOffice.OfficeService.Business.Commands.Office.Find;
using LT.DigitalOffice.OfficeService.Business.Commands.Office.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Office;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.OfficeService.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class OfficeController : ControllerBase
  {
    private readonly IMediator _mediator;
    private readonly IBaseFindFilterValidator _baseFindValidator;

    public OfficeController(
      IMediator mediator,
      IBaseFindFilterValidator baseFindValidator)
    {
      _mediator = mediator;
      _baseFindValidator = baseFindValidator;
    }

    [HttpPost("create")]
    public async Task<OperationResultResponse<Guid>> CreateAsync(
      [FromServices] ICreateOfficeCommand command,
      [FromBody] CreateOfficeRequest request)
    {
      return await command.ExecuteAsync(request);
    }

    [HttpGet("find")]
    public async Task<IActionResult> FindAsync(
      [FromQuery] OfficeFindFilter filter,
      CancellationToken ct)
    {
      if (!_baseFindValidator.ValidateCustom(filter, out List<string> errors) || filter is null)
      {
        return BadRequest(errors);
      }

      return Ok(await _mediator.Send(filter, ct));
    }

    [HttpPatch("edit")]
    public async Task<OperationResultResponse<bool>> EditAsync(
      [FromServices] IEditOfficeCommand command,
      [FromQuery] Guid officeId,
      [FromBody] JsonPatchDocument<EditOfficeRequest> request)
    {
      return await command.ExecuteAsync(officeId, request);
    }
  }
}

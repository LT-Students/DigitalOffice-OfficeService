using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Kernel.Validators.Interfaces;
using LT.DigitalOffice.OfficeService.Business.Commands.Office.Create;
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
    private readonly IAccessValidator _accessValidator;

    public OfficeController(
      IMediator mediator,
      IBaseFindFilterValidator baseFindValidator,
      IAccessValidator accessValidator)
    {
      _mediator = mediator;
      _baseFindValidator = baseFindValidator;
      _accessValidator = accessValidator;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateAsync(
      [FromBody] CreateOfficeRequest request,
      CancellationToken ct)
    {
      if (!await _accessValidator.HasRightsAsync(Rights.AddEditRemoveCompanies))
      {
        return StatusCode(403);
      }

      return Ok(await _mediator.Send(request, ct));
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

using System;
using System.Threading.Tasks;
using LT.DigitalOffice.OfficeService.Business.Commands.Office.Interface;
using LT.DigitalOffice.OfficeService.Models.Dto.Models;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Office;
using LT.DigitalOffice.Kernel.Responses;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Office.Filters;

namespace LT.DigitalOffice.OfficeService.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class OfficeController : ControllerBase
  {
    [HttpPost("create")]
    public async Task<OperationResultResponse<Guid>> CreateAsync(
      [FromServices] ICreateOfficeCommand command,
      [FromBody] CreateOfficeRequest request)
    {
      return await command.ExecuteAsync(request);
    }

    [HttpGet("find")]
    public async Task<FindResultResponse<OfficeInfo>> FindAsync(
      [FromServices] IFindOfficesCommand command,
      [FromQuery] OfficeFindFilter filter)
    {
      return await command.ExecuteAsync(filter);
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

using System;
using System.Threading.Tasks;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Office;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.OfficeService.Business.Commands.Office.Interfaces
{
  [AutoInject]
  public interface IEditOfficeCommand
  {
    Task<OperationResultResponse<bool>> ExecuteAsync(Guid officeId, JsonPatchDocument<EditOfficeRequest> request);
  }
}

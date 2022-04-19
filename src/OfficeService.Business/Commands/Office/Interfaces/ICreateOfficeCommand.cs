using System;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Office;

namespace LT.DigitalOffice.OfficeService.Business.Commands.Office.Interfaces
{
  [AutoInject]
  public interface ICreateOfficeCommand
  {
    Task<OperationResultResponse<Guid>> ExecuteAsync(CreateOfficeRequest request);
  }
}

using System;
using System.Threading.Tasks;

using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.WorkspaceType;

namespace LT.DigitalOffice.OfficeService.Business.Commands.WorkspaceType.Interfaces
{
  [AutoInject]
  public interface ICreateWorkspaceTypeCommand
  {
    Task<OperationResultResponse<Guid?>> ExecuteAsync(CreateWorkspaceTypeRequest request);
  }
}

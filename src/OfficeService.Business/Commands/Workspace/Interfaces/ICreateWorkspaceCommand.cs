using System;
using System.Threading.Tasks;

using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Workspace;

namespace LT.DigitalOffice.OfficeService.Business.Commands.Workspace.Interfaces
{
  [AutoInject]
  public interface ICreateWorkspaceCommand
  {
    Task<OperationResultResponse<Guid?>> ExecuteAsync(CreateWorkspaceRequest request);
  }
}

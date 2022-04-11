using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.OfficeService.Models.Dto.Models.Workspace;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Workspace.Filters;

namespace LT.DigitalOffice.OfficeService.Business.Commands.Workspace.Interfaces
{
  [AutoInject]
  public interface IFindWorkspacesCommand
  {
    Task<FindResultResponse<WorkspaceInfo>> ExecuteAsync(WorkspaceFindFilter filter);
  }
}

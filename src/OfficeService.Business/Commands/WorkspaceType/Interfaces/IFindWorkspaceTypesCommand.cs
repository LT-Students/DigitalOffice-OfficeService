using System.Threading.Tasks;

using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.OfficeService.Models.Dto.Models.Workspace;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.WorkspaceType.Filters;

namespace LT.DigitalOffice.OfficeService.Business.Commands.WorkspaceType.Interfaces
{
  [AutoInject]
  public interface IFindWorkspaceTypesCommand
  {
    Task<FindResultResponse<WorkspaceTypeInfo>> ExecuteAsync(WorkspaceTypeFindFilter filter);
  }
}

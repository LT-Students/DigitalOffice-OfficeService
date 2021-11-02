using System.Threading.Tasks;
using LT.DigitalOffice.OfficeService.Models.Dto.Models;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Office.Filters;

namespace LT.DigitalOffice.OfficeService.Business.Commands.Office.Interface
{
  [AutoInject]
  public interface IFindOfficesCommand
  {
    Task<FindResultResponse<OfficeInfo>> ExecuteAsync(OfficeFindFilter filter);
  }
}

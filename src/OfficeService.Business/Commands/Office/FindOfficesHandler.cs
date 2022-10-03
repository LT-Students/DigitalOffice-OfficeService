using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.OfficeService.Data.Interfaces;
using LT.DigitalOffice.OfficeService.Mappers.Models.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Db;
using LT.DigitalOffice.OfficeService.Models.Dto.Models;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Office.Filters;
using MediatR;

namespace LT.DigitalOffice.OfficeService.Business.Commands.Office
{
  public class FindOfficesHandler : IRequestHandler<OfficeFindFilter, FindResultResponse<OfficeInfo>>
  {
    private readonly IOfficeRepository _officeRepository;
    private readonly IOfficeInfoMapper _mapper;

    public FindOfficesHandler(
      IOfficeRepository officeRepository,
      IOfficeInfoMapper mapper)
    {
      _officeRepository = officeRepository;
      _mapper = mapper;
    }

    public async Task<FindResultResponse<OfficeInfo>> Handle(OfficeFindFilter filter, CancellationToken ct)
    {
      (List<DbOffice> offices, int totalCount) = await _officeRepository.FindAsync(filter);

      return new FindResultResponse<OfficeInfo>
      {
        Body = offices.Select(_mapper.Map).ToList(),
        TotalCount = totalCount
      };
    }
  }
}

using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Kernel.Validators.Interfaces;
using LT.DigitalOffice.OfficeService.Business.Commands.Office.Interfaces;
using LT.DigitalOffice.OfficeService.Data.Interfaces;
using LT.DigitalOffice.OfficeService.Mappers.Models.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Db;
using LT.DigitalOffice.OfficeService.Models.Dto.Models;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Office.Filters;

namespace LT.DigitalOffice.OfficeService.Business.Commands.Office
{
  public class FindOfficesCommand : IFindOfficesCommand
  {
    private readonly IOfficeRepository _officeRepository;
    private readonly IOfficeInfoMapper _mapper;
    private readonly IBaseFindFilterValidator _baseFindValidator;
    private readonly IResponseCreator _responseCreator;

    public FindOfficesCommand(
      IOfficeRepository officeRepository,
      IOfficeInfoMapper mapper,
      IBaseFindFilterValidator baseFindValidator,
      IResponseCreator responseCreator)
    {
      _officeRepository = officeRepository;
      _mapper = mapper;
      _baseFindValidator = baseFindValidator;
      _responseCreator = responseCreator;
    }

    public async Task<FindResultResponse<OfficeInfo>> ExecuteAsync(OfficeFindFilter filter)
    {
      if (!_baseFindValidator.ValidateCustom(filter, out List<string> errors))
      {
        return _responseCreator.CreateFailureFindResponse<OfficeInfo>(HttpStatusCode.BadRequest, errors);
      }

      (List<DbOffice> offices, int totalCount) = await _officeRepository.FindAsync(filter);

      FindResultResponse<OfficeInfo> response = new();

      response.Body = offices
        .Select(_mapper.Map)
        .ToList();

      response.TotalCount = totalCount;

      return response;
    }
  }
}

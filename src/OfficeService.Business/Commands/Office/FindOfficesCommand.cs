using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Kernel.Validators.Interfaces;
using LT.DigitalOffice.OfficeService.Business.Commands.Office.Interface;
using LT.DigitalOffice.OfficeService.Data.Interfaces;
using LT.DigitalOffice.OfficeService.Mappers.Models.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Db;
using LT.DigitalOffice.OfficeService.Models.Dto.Models;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Office.Filters;
using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.OfficeService.Business.Commands.Office
{
  public class FindOfficesCommand : IFindOfficesCommand
  {
    private readonly IOfficeRepository _officeRepository;
    private readonly IOfficeInfoMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IBaseFindFilterValidator _baseFindValidator;


    public FindOfficesCommand(
      IOfficeRepository officeRepository,
      IOfficeInfoMapper mapper,
      IHttpContextAccessor httpContextAccessor,
      IBaseFindFilterValidator baseFindValidator)
    {
      _officeRepository = officeRepository;
      _mapper = mapper;
      _httpContextAccessor = httpContextAccessor;
      _baseFindValidator = baseFindValidator;
    }

    public async Task<FindResultResponse<OfficeInfo>> ExecuteAsync(OfficeFindFilter filter)
    {
      FindResultResponse<OfficeInfo> response = new();

      if (!_baseFindValidator.ValidateCustom(filter, out List<string> errors))
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        response.Status = OperationResultStatusType.Failed;
        response.Errors = errors;
        return response;
      }

      (List<DbOffice> offices, int totalCount) = await _officeRepository.FindAsync(filter);

      response.Body = offices
        .Select(_mapper.Map)
        .ToList();

      response.TotalCount = totalCount;
      response.Status = OperationResultStatusType.FullSuccess;

      return response;
    }
  }
}

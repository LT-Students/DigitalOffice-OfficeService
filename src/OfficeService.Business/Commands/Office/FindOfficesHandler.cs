using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.OfficeService.Business.Commands.Office.Interfaces;
using LT.DigitalOffice.OfficeService.Data.Provider;
using LT.DigitalOffice.OfficeService.Mappers.Models.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Db;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.OfficeService.Business.Commands.Office
{
  public class FindOfficesHandler : IRequestHandler<OfficeFindFilter, FindResultResponse<OfficeInfo>>
  {
    private readonly IDataProvider _provider;
    private readonly IOfficeInfoMapper _mapper;

    public FindOfficesHandler(
      IDataProvider provider,
      IOfficeInfoMapper mapper)
    {
      _provider = provider;
      _mapper = mapper;
    }

    public async Task<FindResultResponse<OfficeInfo>> Handle(OfficeFindFilter filter, CancellationToken ct)
    {
      IQueryable<DbOffice> dbOffices = _provider.Offices.AsQueryable();

      if (filter.IsActive.HasValue)
      {
        dbOffices = dbOffices.Where(x => x.IsActive == filter.IsActive);
      }

      if (!string.IsNullOrWhiteSpace(filter.NameIncludeSubstring))
      {
        dbOffices = dbOffices.Where(x =>
          x.Name.Contains(filter.NameIncludeSubstring));
      }

      if (filter.IsAscendingSort.HasValue)
      {
        dbOffices = filter.IsAscendingSort.Value
          ? dbOffices.OrderBy(o => o.Name)
          : dbOffices.OrderByDescending(o => o.Name);
      }

      return new FindResultResponse<OfficeInfo>
      {
        TotalCount = await dbOffices.CountAsync(ct),
        Body = dbOffices
          .Skip(filter.SkipCount)
          .Take(filter.TakeCount)
          .AsEnumerable()
          .Select(_mapper.Map)
          .ToList()
      };
    }
  }
}

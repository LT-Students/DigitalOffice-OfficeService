using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.OfficeService.Data.Provider;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.OfficeService.Business.Commands.Office.Find
{
  public class FindOfficesHandler : IRequestHandler<OfficeFindFilter, FindResultResponse<OfficeInfo>>
  {
    private readonly IDataProvider _provider;

    public FindOfficesHandler(
      IDataProvider provider)
    {
      _provider = provider;
    }

    public async Task<FindResultResponse<OfficeInfo>> Handle(OfficeFindFilter filter, CancellationToken ct)
    {
      IQueryable<OfficeInfo> offices = _provider.Offices.Select(o => new OfficeInfo
      {
        Id = o.Id,
        Name = o.Name,
        City = o.City,
        Address = o.Address,
        Latitude = o.Latitude,
        Longitude = o.Longitude,
        IsActive = o.IsActive
      });

      if (filter.IsActive.HasValue)
      {
        offices = offices.Where(x => x.IsActive == filter.IsActive);
      }

      if (!string.IsNullOrWhiteSpace(filter.NameIncludeSubstring))
      {
        offices = offices.Where(x =>
          x.Name.Contains(filter.NameIncludeSubstring));
      }

      if (filter.IsAscendingSort.HasValue)
      {
        offices = filter.IsAscendingSort.Value
          ? offices.OrderBy(o => o.Name)
          : offices.OrderByDescending(o => o.Name);
      }

      return new FindResultResponse<OfficeInfo>
      {
        TotalCount = await offices.CountAsync(ct),
        Body = await offices
          .Skip(filter.SkipCount)
          .Take(filter.TakeCount)
          .ToListAsync(ct)
      };
    }
  }
}

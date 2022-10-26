﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Validators.Interfaces;
using LT.DigitalOffice.OfficeService.DataLayer;
using LT.DigitalOffice.OfficeService.DataLayer.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.OfficeService.Business.Office.Find
{
  public class FindOfficesHandler : IRequestHandler<OfficeFindFilter, FindResult<OfficeInfo>>
  {
    private readonly OfficeServiceDbContext _dbContext;
    private readonly IBaseFindFilterValidator _baseFindValidator;

    #region private methods

    private async Task<(List<DbOffice>, int totalCount)> FindOfficesAsync(
      OfficeFindFilter filter,
      CancellationToken ct)
    {
      IQueryable<DbOffice> dbOffices = _dbContext.Offices
        .AsQueryable();

      if (filter.IsActive.HasValue)
      {
        dbOffices = dbOffices.Where(x => x.IsActive == filter.IsActive);
      }

      if (!string.IsNullOrWhiteSpace(filter.NameIncludeSubstring))
      {
        dbOffices = dbOffices.Where(x =>
          x.Name.ToLower().Contains(filter.NameIncludeSubstring.ToLower()));
      }

      if (filter.IsAscendingSort.HasValue)
      {
        dbOffices = filter.IsAscendingSort.Value
          ? dbOffices.OrderBy(o => o.Name)
          : dbOffices.OrderByDescending(o => o.Name);
      }

      return (await dbOffices
        .Skip(filter.SkipCount)
        .Take(filter.TakeCount)
        .ToListAsync(ct),
        await dbOffices.CountAsync(ct));
    }

    private OfficeInfo Map(DbOffice office)
    {
      return new OfficeInfo
      {
        Id = office.Id,
        Name = office.Name,
        City = office.City,
        Address = office.Address,
        Latitude = office.Latitude,
        Longitude = office.Longitude,
        IsActive = office.IsActive
      };
    }

    #endregion

    public FindOfficesHandler(
      OfficeServiceDbContext dbContext,
      IBaseFindFilterValidator baseFindValidator)
    {
      _dbContext = dbContext;
      _baseFindValidator = baseFindValidator;
    }

    public async Task<FindResult<OfficeInfo>> Handle(OfficeFindFilter filter, CancellationToken ct)
    {
      if (!_baseFindValidator.ValidateCustom(filter, out List<string> errors) || filter is null)
      {
        throw new BadRequestException(errors);
      }

      (List<DbOffice> offices, int totalCount) result = await FindOfficesAsync(filter, ct);

      return new FindResult<OfficeInfo>
      {
        Body = result.offices.ConvertAll(Map),
        TotalCount = result.totalCount
      };
    }
  }
}

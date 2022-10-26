using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.OfficeService.DataLayer;
using LT.DigitalOffice.OfficeService.DataLayer.Models;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.OfficeService.Business.Office.Create
{
  public class CreateOfficeHandler : IRequestHandler<CreateOfficeRequest, Guid?>
  {
    private readonly OfficeServiceDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    #region private methods

    private DbOffice Map(CreateOfficeRequest request)
    {
      Regex nameRegex = new(@"^\s+|\s+$|\s+(?=\s)");

      return new DbOffice
      {
        Id = Guid.NewGuid(),
        Name = !string.IsNullOrWhiteSpace(request.Name) ? nameRegex.Replace(request.Name, "") : null,
        City = request.City.Trim(),
        Address = request.Address.Trim(),
        Latitude = request.Latitude,
        Longitude = request.Longitude,
        CreatedAtUtc = DateTime.UtcNow,
        CreatedBy = _httpContextAccessor.HttpContext.GetUserId(),
        IsActive = true
      };
    }

    private async Task<Guid?> CreateOfficeAsync(DbOffice office, CancellationToken ct)
    {
      await _dbContext.Offices.AddAsync(office, ct);
      await _dbContext.SaveAsync();

      return office.Id;
    }

    #endregion

    public CreateOfficeHandler(
      OfficeServiceDbContext dbContext,
      IHttpContextAccessor httpContextAccessor)
    {
      _dbContext = dbContext;
      _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Guid?> Handle(CreateOfficeRequest request, CancellationToken ct)
    {
      return await CreateOfficeAsync(Map(request), ct);
    }
  }
}

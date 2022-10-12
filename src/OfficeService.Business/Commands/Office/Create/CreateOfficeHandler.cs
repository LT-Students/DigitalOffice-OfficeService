using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.OfficeService.Data.Provider;
using LT.DigitalOffice.OfficeService.Models.Db;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.OfficeService.Business.Commands.Office.Create
{
  public class CreateOfficeHandler : IRequestHandler<CreateOfficeRequest, Guid?>
  {
    private readonly Regex _nameRegex = new(@"^\s+|\s+$|\s+(?=\s)");
    private readonly IDataProvider _provider;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateOfficeHandler(
      IDataProvider provider,
      IHttpContextAccessor httpContextAccessor)
    {
      _provider = provider;
      _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Guid?> Handle(CreateOfficeRequest request, CancellationToken ct)
    {
      DbOffice office = new()
      {
        Id = Guid.NewGuid(),
        Name = !string.IsNullOrWhiteSpace(request.Name) ? _nameRegex.Replace(request.Name, "") : null,
        City = request.City.Trim(),
        Address = request.Address.Trim(),
        Latitude = request.Latitude,
        Longitude = request.Longitude,
        CreatedAtUtc = DateTime.UtcNow,
        CreatedBy = _httpContextAccessor.HttpContext.GetUserId(),
        IsActive = true
      };

      await _provider.Offices.AddAsync(office, ct);
      await _provider.SaveAsync();

      return office.Id;
    }
  }
}

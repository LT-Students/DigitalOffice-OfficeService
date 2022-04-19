using System;

namespace LT.DigitalOffice.OfficeService.Models.Dto.Models.Workspace
{
  public record TagInfo
  {
    public Guid Id { get; set; }

    public string Name { get; set; }
  }
}

using System;

namespace LT.DigitalOffice.OfficeService.Models.Dto.Models.Workspace
{
  public record ImageInfo
  {
    public Guid Id { get; set; }

    public Guid? ParentId { get; set; }

    public string Content { get; set; }

    public string Extension { get; set; }

    public string Name { get; set; }
  }
}

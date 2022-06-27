using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.OfficeService.Models.Dto.Requests.Users
{
  public record CreateOfficeUsers
  {
    public Guid OfficeId { get; set; }
    public List<Guid> UsersIds { get; set; }
  }
}

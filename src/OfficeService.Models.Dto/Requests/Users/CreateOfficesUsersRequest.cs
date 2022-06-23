using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.OfficeService.Models.Dto.Requests.Users
{
  public record CreateOfficesUsersRequest
  {
    public Guid OfficeId { get; set; }
    public List<Guid> UsersIds { get; set; }
  }
}

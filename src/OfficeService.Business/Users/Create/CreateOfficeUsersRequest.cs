using System;
using System.Collections.Generic;
using MediatR;

namespace LT.DigitalOffice.OfficeService.Business.Users.Create
{
  public record CreateOfficeUsersRequest : IRequest<bool>
  {
    public Guid OfficeId { get; set; }
    public List<Guid> UsersIds { get; set; }
  }
}

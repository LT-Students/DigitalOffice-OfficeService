using System;
using System.Collections.Generic;
using MediatR;

namespace LT.DigitalOffice.OfficeService.Business.Commands.Users.Remove
{
  public record RemoveOfficeUsersRequest : IRequest<bool>
  {
    public Guid OfficeId { get; set; }
    public List<Guid> UsersIds { get; set; }
  }
}

﻿using System;

namespace LT.DigitalOffice.OfficeService.Models.Dto.Requests.Users
{
  public record ChangeOfficeRequest
  {
    public Guid? OfficeId { get; set; }
    public Guid UserId { get; set; }
  }
}

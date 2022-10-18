﻿using System;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.OfficeService.Business.Commands.Office.Edit
{
  public class EditOfficeRequest : IRequest<bool>
  {
    public Guid OfficeId { get; set; }
    public JsonPatchDocument<EditOfficePatch> Patch { get; set; }
  }
}
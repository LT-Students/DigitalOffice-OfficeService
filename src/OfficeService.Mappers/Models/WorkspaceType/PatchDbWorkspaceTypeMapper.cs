using System;
using LT.DigitalOffice.OfficeService.Mappers.Models.WorkspaceType.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Db;
using LT.DigitalOffice.OfficeService.Models.Dto.Enums.Workspace;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.WorkspaceType;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace LT.DigitalOffice.OfficeService.Mappers.Models.WorkspaceType;

public class PatchDbWorkspaceTypeMapper : IPatchDbWorkspaceTypeMapper
{
  public JsonPatchDocument<DbWorkspaceType> Map(JsonPatchDocument<EditWorkspaceTypeRequest> request)
  {
    if (request is null)
    {
      return null;
    }

    JsonPatchDocument<DbWorkspaceType> patchDbWorkspaceType = new();

    foreach (Operation<EditWorkspaceTypeRequest> item in request.Operations)
    {
      if (item.path.EndsWith(nameof(EditWorkspaceTypeRequest.BookingRule), StringComparison.OrdinalIgnoreCase))
      {
        patchDbWorkspaceType.Operations.Add(new Operation<DbWorkspaceType>(item.op, item.path, item.from, (int)Enum.Parse<BookingRule>(item.value?.ToString())));
        continue;
      }

      patchDbWorkspaceType.Operations.Add(new Operation<DbWorkspaceType>(item.op, item.path, item.from, item.value));
    }

    return patchDbWorkspaceType;
  }
}

using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.OfficeService.Models.Db;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.WorkspaceType;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.OfficeService.Mappers.Models.WorkspaceType.Interfaces;

[AutoInject]
public interface IPatchDbWorkspaceTypeMapper
{
  JsonPatchDocument<DbWorkspaceType> Map(JsonPatchDocument<EditWorkspaceTypeRequest> request);
}

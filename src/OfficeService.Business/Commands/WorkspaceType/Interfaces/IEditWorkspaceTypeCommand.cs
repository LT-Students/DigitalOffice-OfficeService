using System;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.WorkspaceType;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.OfficeService.Business.Commands.WorkspaceType.Interfaces;

[AutoInject]
public interface IEditWorkspaceTypeCommand
{
  Task<OperationResultResponse<bool>> Execute(Guid workspaceTypeId, JsonPatchDocument<EditWorkspaceTypeRequest> request);
}

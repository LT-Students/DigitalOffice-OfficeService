using FluentValidation;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.WorkspaceType;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.OfficeService.Validation.WorkspaceType.Interfaces;

[AutoInject]
public interface IEditWorkspaceTypeRequestValidator : IValidator<JsonPatchDocument<EditWorkspaceTypeRequest>>
{
}

using FluentValidation;
using LT.DigitalOffice.Kernel.Attributes;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.OfficeService.Business.Commands.WorkspaceType.Edit;

[AutoInject]
public interface IEditWorkspaceTypeValidator : IValidator<JsonPatchDocument<EditWorkspaceTypePatch>>
{
}

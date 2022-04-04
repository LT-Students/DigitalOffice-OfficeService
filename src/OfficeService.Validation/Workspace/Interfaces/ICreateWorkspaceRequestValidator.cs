using FluentValidation;

using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Workspace;

namespace LT.DigitalOffice.OfficeService.Validation.Workspace.Interfaces
{
  [AutoInject]
  public interface ICreateWorkspaceRequestValidator : IValidator<CreateWorkspaceRequest>
  {
  }
}

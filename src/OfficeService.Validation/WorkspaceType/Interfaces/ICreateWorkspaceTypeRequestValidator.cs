using FluentValidation;

using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.WorkspaceType;

namespace LT.DigitalOffice.OfficeService.Validation.WorkspaceType.Interfaces
{
  [AutoInject]
  public interface ICreateWorkspaceTypeRequestValidator : IValidator<CreateWorkspaceTypeRequest>
  {
  }
}

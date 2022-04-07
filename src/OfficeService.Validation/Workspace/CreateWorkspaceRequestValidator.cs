using FluentValidation;

using LT.DigitalOffice.OfficeService.Data.Workspace.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Workspace;
using LT.DigitalOffice.OfficeService.Validation.Workspace.Interfaces;

namespace LT.DigitalOffice.OfficeService.Validation.Workspace
{
  public class CreateWorkspaceRequestValidator : AbstractValidator<CreateWorkspaceRequest>, ICreateWorkspaceRequestValidator
  {
    public CreateWorkspaceRequestValidator(IWorkspaceRepository repository)
    {
      RuleFor(w => w.Name)
        .NotEmpty().WithMessage("Workspace name cannot be empty");
    }
  }
}

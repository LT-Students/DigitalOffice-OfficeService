using FluentValidation;

namespace LT.DigitalOffice.OfficeService.Business.Workspace.Create
{
  public class CreateWorkspaceValidator : AbstractValidator<CreateWorkspaceRequest>
  {
    public CreateWorkspaceValidator()
    {
      RuleFor(w => w.Name)
        .NotEmpty().WithMessage("Workspace name cannot be empty");
    }
  }
}

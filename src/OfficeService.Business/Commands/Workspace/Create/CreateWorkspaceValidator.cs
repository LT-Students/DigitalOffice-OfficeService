using FluentValidation;

namespace LT.DigitalOffice.OfficeService.Business.Commands.Workspace.Create
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

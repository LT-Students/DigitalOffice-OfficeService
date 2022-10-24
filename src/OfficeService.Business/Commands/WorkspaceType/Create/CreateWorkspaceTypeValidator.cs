using FluentValidation;
using LT.DigitalOffice.OfficeService.Data.Provider;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.OfficeService.Business.Commands.WorkspaceType.Create
{
  public class CreateWorkspaceTypeValidator : AbstractValidator<CreateWorkspaceTypeRequest>
  {
    public CreateWorkspaceTypeValidator(IDataProvider provider)
    {
      RuleFor(wt => wt.Name)
        .Cascade(CascadeMode.Stop)
        .NotEmpty().WithMessage("Workspace type name cannot be empty")
        .MaximumLength(100).WithMessage("Workspace type name length cannot be more than 100 characters")
        .MustAsync(async (n, _) => !await provider.WorkspacesTypes.AnyAsync(wt => string.Equals(wt.Name, n)));

      RuleFor(wt => wt.Description)
        .MaximumLength(300).WithMessage("Workspace type description length cannot be more than 300 characters");

      RuleFor(wt => wt.BookingRule)
        .IsInEnum().WithMessage("Incorrect booking rule for workspace type");
    }
  }
}

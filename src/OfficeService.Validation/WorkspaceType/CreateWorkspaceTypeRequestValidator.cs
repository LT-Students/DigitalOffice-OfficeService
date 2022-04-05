using FluentValidation;

using LT.DigitalOffice.OfficeService.Data.WorkspaceType.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.WorkspaceType;
using LT.DigitalOffice.OfficeService.Validation.WorkspaceType.Interfaces;

namespace LT.DigitalOffice.OfficeService.Validation.WorkspaceType
{
  public class CreateWorkspaceTypeRequestValidator : AbstractValidator<CreateWorkspaceTypeRequest>, ICreateWorkspaceTypeRequestValidator
  {
    public CreateWorkspaceTypeRequestValidator(IWorkspaceTypeRepository repository)
    {
      RuleFor(wt => wt.Name)
        .NotEmpty().WithMessage("Workspace type name cannot be empty")
        .NotNull().WithMessage("Workspace type name cannot be null")
        .MaximumLength(100).WithMessage("Workspace type name length cannot be more than 100 characters");

      RuleFor(wt => wt.Description)
        .MaximumLength(300).WithMessage("Workspace type description length cannot be more than 300 characters");

      RuleFor(wt => wt.BookingRule)
        .Must(br => br is <= 2 and >= 0).WithMessage("Incorrect booking rule for workspace type");
    }
  }
}

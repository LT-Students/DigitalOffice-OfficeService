using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
      // TODO: Add rules
    }
  }
}

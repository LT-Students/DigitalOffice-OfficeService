using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentValidation;
using LT.DigitalOffice.OfficeService.Data.Workspace.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Office;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Workspace;
using LT.DigitalOffice.OfficeService.Validation.Office.Interfaces;
using LT.DigitalOffice.OfficeService.Validation.Workspace.Interfaces;

namespace LT.DigitalOffice.OfficeService.Validation.Workspace
{
  public class CreateWorkspaceRequestValidator : AbstractValidator<CreateWorkspaceRequest>, ICreateWorkspaceRequestValidator
  {
    private readonly Regex _nameRegex = new(@"^\s+|\s+$|\s+(?=\s)");

    public CreateWorkspaceRequestValidator(IWorkspaceRepository repository)
    {
      // TODO: Add rules
    }
  }
}

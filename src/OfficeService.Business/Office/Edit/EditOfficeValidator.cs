using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentValidation;
using LT.DigitalOffice.Kernel.Validators;
using LT.DigitalOffice.OfficeService.DataLayer;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.OfficeService.Business.Office.Edit
{
  public class EditOfficeValidator : BaseEditRequestValidator<EditOfficePatch>, IEditOfficeValidator
  {
    private readonly Regex _nameRegex = new(@"^\s+|\s+$|\s+(?=\s)");
    private readonly OfficeServiceDbContext _dbContext;

    private async Task<bool> DoesNameExistAsync(string name)
    {
      return !await _dbContext.Offices.AnyAsync(o => o.Name == name);
    }

    private void HandleInternalPropertyValidation(
      Operation<EditOfficePatch> requestedOperation,
      ValidationContext<JsonPatchDocument<EditOfficePatch>> context)
    {
      Context = context;
      RequestedOperation = requestedOperation;

      #region Paths

      AddСorrectPaths(
        new()
        {
          nameof(EditOfficePatch.Name),
          nameof(EditOfficePatch.City),
          nameof(EditOfficePatch.Address),
          nameof(EditOfficePatch.Longitude),
          nameof(EditOfficePatch.Latitude),
          nameof(EditOfficePatch.IsActive),
        });

      AddСorrectOperations(nameof(EditOfficePatch.Name), new() { OperationType.Replace });
      AddСorrectOperations(nameof(EditOfficePatch.City), new() { OperationType.Replace });
      AddСorrectOperations(nameof(EditOfficePatch.Address), new() { OperationType.Replace });
      AddСorrectOperations(nameof(EditOfficePatch.Longitude), new() { OperationType.Replace });
      AddСorrectOperations(nameof(EditOfficePatch.Latitude), new() { OperationType.Replace });
      AddСorrectOperations(nameof(EditOfficePatch.IsActive), new() { OperationType.Replace });

      #endregion

      #region string property

      AddFailureForPropertyIf(
        nameof(EditOfficePatch.City),
        x => x == OperationType.Replace,
        new()
        {
          { x => !string.IsNullOrEmpty(x.value?.ToString()), "City cannot be empty." },
          { x => x.value?.ToString().Length < 201, "City's name is too long." }
        },
        CascadeMode.Stop);

      AddFailureForPropertyIf(
        nameof(EditOfficePatch.Address),
        x => x == OperationType.Replace,
        new()
        {
          { x => !string.IsNullOrEmpty(x.value?.ToString().Trim()), "Address cannot be empty." }
        });

      #endregion

      #region IsActive

      AddFailureForPropertyIf(
        nameof(EditOfficePatch.IsActive),
        x => x == OperationType.Replace,
        new()
        {
          { x => bool.TryParse(x.value?.ToString(), out _), "Incorrect IsActive value." }
        });

      #endregion
    }

    public EditOfficeValidator(OfficeServiceDbContext dbContext)
    {
      _dbContext = dbContext;

      RuleForEach(x => x.Operations)
        .Custom(HandleInternalPropertyValidation);

      When(x => !string.IsNullOrWhiteSpace(
        x.Operations.FirstOrDefault(o => o.path.EndsWith(nameof(EditOfficePatch.Name), StringComparison.OrdinalIgnoreCase))?.value?.ToString()),
        () =>
        {
          RuleFor(patch => patch)
            .MustAsync(async (patch, _) =>
            {
              return await DoesNameExistAsync(_nameRegex.Replace(patch.Operations.FirstOrDefault(
                    op => op.path.EndsWith(nameof(EditOfficePatch.Name), StringComparison.OrdinalIgnoreCase)).value
                  .ToString(), ""));
            })
            .WithMessage("Name already exists.");
        });
    }
  }
}

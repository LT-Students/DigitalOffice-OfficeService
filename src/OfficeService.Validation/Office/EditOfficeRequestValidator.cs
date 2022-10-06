using System;
using System.Linq;
using System.Text.RegularExpressions;
using FluentValidation;
using LT.DigitalOffice.Kernel.Validators;
using LT.DigitalOffice.OfficeService.Data.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Office;
using LT.DigitalOffice.OfficeService.Validation.Office.Interfaces;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace LT.DigitalOffice.OfficeService.Validation.Office
{
  public class EditOfficeRequestValidator : BaseEditRequestValidator<EditOfficeRequest>, IEditOfficeRequestValidator
  {
    private readonly Regex _nameRegex = new(@"^\s+|\s+$|\s+(?=\s)");

    private void HandleInternalPropertyValidation(
      Operation<EditOfficeRequest> requestedOperation,
      ValidationContext<JsonPatchDocument<EditOfficeRequest>> context)
    {
      Context = context;
      RequestedOperation = requestedOperation;

      #region Paths

      AddСorrectPaths(
        new()
        {
          nameof(EditOfficeRequest.Name),
          nameof(EditOfficeRequest.City),
          nameof(EditOfficeRequest.Address),
          nameof(EditOfficeRequest.Longitude),
          nameof(EditOfficeRequest.Latitude),
          nameof(EditOfficeRequest.IsActive),
        });

      AddСorrectOperations(nameof(EditOfficeRequest.Name), new() { OperationType.Replace });
      AddСorrectOperations(nameof(EditOfficeRequest.City), new() { OperationType.Replace });
      AddСorrectOperations(nameof(EditOfficeRequest.Address), new() { OperationType.Replace });
      AddСorrectOperations(nameof(EditOfficeRequest.Longitude), new() { OperationType.Replace });
      AddСorrectOperations(nameof(EditOfficeRequest.Latitude), new() { OperationType.Replace });
      AddСorrectOperations(nameof(EditOfficeRequest.IsActive), new() { OperationType.Replace });

      #endregion

      #region string property

      AddFailureForPropertyIf(
        nameof(EditOfficeRequest.City),
        x => x == OperationType.Replace,
        new()
        {
          { x => !string.IsNullOrEmpty(x.value?.ToString()), "City cannot be empty." },
          { x => x.value?.ToString().Length < 201, "City's name is too long." }
        },
        CascadeMode.Stop);

      AddFailureForPropertyIf(
        nameof(EditOfficeRequest.Address),
        x => x == OperationType.Replace,
        new()
        {
          { x => !string.IsNullOrEmpty(x.value?.ToString().Trim()), "Address cannot be empty." }
        });

      #endregion

      #region IsActive

      AddFailureForPropertyIf(
        nameof(EditOfficeRequest.IsActive),
        x => x == OperationType.Replace,
        new()
        {
          { x => bool.TryParse(x.value?.ToString(), out _), "Incorrect IsActive value." }
        });

      #endregion
    }

    public EditOfficeRequestValidator(
      IOfficeRepository _officeRepository)
    {
      RuleForEach(x => x.Operations)
        .Custom(HandleInternalPropertyValidation);

      When(x => !string.IsNullOrWhiteSpace(
        x.Operations.FirstOrDefault(o => o.path.EndsWith(nameof(EditOfficeRequest.Name), StringComparison.OrdinalIgnoreCase))?.value?.ToString()),
        () =>
        {
          RuleFor(patch => patch)
            .MustAsync(async (patch, _) =>
              {
                return await _officeRepository.DoesNameExistAsync(
                  _nameRegex.Replace(patch.Operations.FirstOrDefault(
                    o => o.path.EndsWith(nameof(EditOfficeRequest.Name), StringComparison.OrdinalIgnoreCase)).value?.ToString(), ""));
              })
            .WithMessage("Name already exists.");
        });
    }
  }
}

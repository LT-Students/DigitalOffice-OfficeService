using System;
using System.Linq;
using System.Text.RegularExpressions;
using FluentValidation;
using FluentValidation.Validators;
using LT.DigitalOffice.Kernel.Validators;
using LT.DigitalOffice.OfficeService.Data.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Db;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Office;
using LT.DigitalOffice.OfficeService.Validation.Office.Interfaces;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace LT.DigitalOffice.OfficeService.Validation.Office
{
  public class EditOfficeRequestValidator : ExtendedEditRequestValidator<Guid, EditOfficeRequest>, IEditOfficeRequestValidator
  {
    private readonly Regex _nameRegex = new(@"^\s+|\s+$|\s+(?=\s)");

    private void HandleInternalPropertyValidation(
      Operation<EditOfficeRequest> requestedOperation,
      CustomContext context)
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
        });

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
      RuleForEach(x => x.Item2.Operations)
        .Custom(HandleInternalPropertyValidation);

      When(x => !string.IsNullOrWhiteSpace(
        x.Item2.Operations.FirstOrDefault(o => o.path.EndsWith(nameof(EditOfficeRequest.Name), StringComparison.OrdinalIgnoreCase))?.value?.ToString()),
        () =>
        {
          RuleFor(tuple => tuple)
            .MustAsync(async (tuple, _) =>
              {
                return await _officeRepository.IsNameUniqueAsync(
                  tuple.Item1,
                  _nameRegex.Replace(tuple.Item2.Operations.FirstOrDefault(
                    o => o.path.EndsWith(nameof(EditOfficeRequest.Name), StringComparison.OrdinalIgnoreCase)).value?.ToString(), ""));
              })
            .WithMessage("Name must be unique.");
        });

      When(x => bool.TryParse(x.Item2.Operations.FirstOrDefault(
          o => o.path.EndsWith(nameof(EditOfficeRequest.IsActive), StringComparison.OrdinalIgnoreCase))?.value?.ToString(), out bool isActive)
        && isActive,
        () =>
        {
          RuleFor(tuple => tuple)
            .MustAsync(async (tuple, _) =>
            {
              DbOffice dbOffice = await _officeRepository.GetAsync(tuple.Item1);

              if (!dbOffice.IsActive && !tuple.Item2.Operations.Any(o => o.path.EndsWith(nameof(EditOfficeRequest.Name), StringComparison.OrdinalIgnoreCase)))
              {
                return await _officeRepository.IsNameUniqueAsync(dbOffice.Id, dbOffice.Name);
              }

              return true;
            })
            .WithMessage("Name must be unique.");
        });
    } 
  }
}

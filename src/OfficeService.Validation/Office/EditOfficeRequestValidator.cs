﻿using FluentValidation;
using FluentValidation.Validators;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Office;
using LT.DigitalOffice.OfficeService.Validation.Office.Interfaces;
using LT.DigitalOffice.Kernel.Validators;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace LT.DigitalOffice.OfficeService.Validation.Office
{
  public class EditOfficeRequestValidator : BaseEditRequestValidator<EditOfficeRequest>, IEditOfficeRequestValidator
  {
    private void HandleInternalPropertyValidation(Operation<EditOfficeRequest> requestedOperation, CustomContext context)
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
        });

      AddСorrectOperations(nameof(EditOfficeRequest.Name), new() { OperationType.Replace });
      AddСorrectOperations(nameof(EditOfficeRequest.City), new() { OperationType.Replace });
      AddСorrectOperations(nameof(EditOfficeRequest.Address), new() { OperationType.Replace });
      AddСorrectOperations(nameof(EditOfficeRequest.Longitude), new() { OperationType.Replace });
      AddСorrectOperations(nameof(EditOfficeRequest.Latitude), new() { OperationType.Replace });

      #endregion

      #region string property

      AddFailureForPropertyIf(
        nameof(EditOfficeRequest.City),
        x => x == OperationType.Replace,
        new()
        {
          { x => !string.IsNullOrEmpty(x.value?.ToString()), "City cannot be empty." },
        });

      AddFailureForPropertyIf(
        nameof(EditOfficeRequest.Address),
        x => x == OperationType.Replace,
        new()
        {
          { x => !string.IsNullOrEmpty(x.value?.ToString()), "Address cannot be empty." },
        });

      #endregion
    }

    public EditOfficeRequestValidator()
    {
      RuleForEach(x => x.Operations)
        .Custom(HandleInternalPropertyValidation);
    }
  }
}

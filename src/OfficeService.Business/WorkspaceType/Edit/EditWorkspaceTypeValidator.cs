using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using LT.DigitalOffice.Kernel.Validators;
using LT.DigitalOffice.OfficeService.Broker.Requests;
using LT.DigitalOffice.OfficeService.DataLayer;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.OfficeService.Business.WorkspaceType.Edit;

public class EditWorkspaceTypeValidator : BaseEditRequestValidator<EditWorkspaceTypePatch>, IEditWorkspaceTypeValidator
{
  private readonly IDataProvider _provider;

  private async Task HandleInternalPropertyValidationAsync(Operation<EditWorkspaceTypePatch> requestedOperation, ValidationContext<JsonPatchDocument<EditWorkspaceTypePatch>> context)
  {
    Context = context;
    RequestedOperation = requestedOperation;

    #region paths

    AddСorrectPaths(
      new List<string>
      {
        nameof(EditWorkspaceTypePatch.Name),
        nameof(EditWorkspaceTypePatch.Description),
        nameof(EditWorkspaceTypePatch.StartTime),
        nameof(EditWorkspaceTypePatch.EndTime),
        nameof(EditWorkspaceTypePatch.BookingRule),
        nameof(EditWorkspaceTypePatch.IsActive)
      });

    AddСorrectOperations(nameof(EditWorkspaceTypePatch.Name), new List<OperationType> { OperationType.Replace });
    AddСorrectOperations(nameof(EditWorkspaceTypePatch.Description), new List<OperationType> { OperationType.Replace });
    AddСorrectOperations(nameof(EditWorkspaceTypePatch.StartTime), new List<OperationType> { OperationType.Replace });
    AddСorrectOperations(nameof(EditWorkspaceTypePatch.EndTime), new List<OperationType> { OperationType.Replace });
    AddСorrectOperations(nameof(EditWorkspaceTypePatch.BookingRule), new List<OperationType> { OperationType.Replace });
    AddСorrectOperations(nameof(EditWorkspaceTypePatch.IsActive), new List<OperationType> { OperationType.Replace });

    #endregion

    #region Name

    await AddFailureForPropertyIfAsync(
      nameof(EditWorkspaceTypePatch.Name),
      x => x == OperationType.Replace,
      new()
      {
        {
          x => Task.FromResult(!string.IsNullOrWhiteSpace(x.value?.ToString())),
          "Workspace type name cannot be empty."
        },
        {
          x => Task.FromResult(x.value.ToString().Trim().Length < 101),
          "Workspace type name length cannot be more than 100 characters."
        },
        {
          async x => !await _provider.WorkspacesTypes.AnyAsync(wt => string.Equals(wt.Name, x.value.ToString().Trim())),
          "Workspace type name already exists."
        }
      }, CascadeMode.Stop);

    #endregion

    #region Description

    AddFailureForPropertyIf(
      nameof(EditWorkspaceTypePatch.Description),
      x => x == OperationType.Replace,
      new()
      {
        {
          x => string.IsNullOrEmpty(x.value?.ToString()) || x.value.ToString().Trim().Length < 301,
          "Workspace type description length cannot be more than 300 characters."
        }
      });

    #endregion

    #region StartTime

    AddFailureForPropertyIf(
      nameof(EditWorkspaceTypePatch.StartTime),
      x => x == OperationType.Replace,
      new()
      {
        {
          x => string.IsNullOrEmpty(x.value?.ToString()) || DateTime.TryParse(x.value.ToString(), out _),
          "Start time has incorrect format."
        }
      });

    #endregion

    #region EndTime

    AddFailureForPropertyIf(
      nameof(EditWorkspaceTypePatch.EndTime),
      x => x == OperationType.Replace,
      new()
      {
        {
          x => string.IsNullOrEmpty(x.value?.ToString()) || DateTime.TryParse(x.value.ToString(), out _),
          "End time has incorrect format."
        }
      });

    #endregion

    #region BookingRule

    AddFailureForPropertyIf(
      nameof(EditWorkspaceTypePatch.BookingRule),
      x => x == OperationType.Replace,
      new()
      {
        {
          x => Enum.TryParse(typeof(BookingRule), x.value?.ToString(), true, out _),
          "Incorrect booking rule."
        }
      });

    #endregion

    #region IsActive

    AddFailureForPropertyIf(
      nameof(EditWorkspaceTypePatch.IsActive),
      x => x == OperationType.Replace,
      new()
      {
        {
          x => bool.TryParse(x.value?.ToString(), out _),
          "Incorrect IsActive value."
        }
      });

    #endregion

  }

  public EditWorkspaceTypeValidator(
    IDataProvider provider)
  {
    _provider = provider;

    RuleForEach(x => x.Operations)
      .CustomAsync(async (x, context, _) => await HandleInternalPropertyValidationAsync(x, context));
  }
}

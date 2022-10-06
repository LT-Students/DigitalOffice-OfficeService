using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using LT.DigitalOffice.Kernel.Validators;
using LT.DigitalOffice.OfficeService.Data.WorkspaceType.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Dto.Enums.Workspace;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.WorkspaceType;
using LT.DigitalOffice.OfficeService.Validation.WorkspaceType.Interfaces;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace LT.DigitalOffice.OfficeService.Validation.WorkspaceType;

public class EditWorkspaceTypeRequestValidator : BaseEditRequestValidator<EditWorkspaceTypeRequest>, IEditWorkspaceTypeRequestValidator
{
  private readonly IWorkspaceTypeRepository _workspaceTypeRepository;

  private async Task HandleInternalPropertyValidationAsync(Operation<EditWorkspaceTypeRequest> requestedOperation, ValidationContext<JsonPatchDocument<EditWorkspaceTypeRequest>> context)
  {
    Context = context;
    RequestedOperation = requestedOperation;

    #region paths

    AddСorrectPaths(
      new List<string>
      {
        nameof(EditWorkspaceTypeRequest.Name),
        nameof(EditWorkspaceTypeRequest.Description),
        nameof(EditWorkspaceTypeRequest.StartTime),
        nameof(EditWorkspaceTypeRequest.EndTime),
        nameof(EditWorkspaceTypeRequest.BookingRule),
        nameof(EditWorkspaceTypeRequest.IsActive)
      });

    AddСorrectOperations(nameof(EditWorkspaceTypeRequest.Name), new List<OperationType> { OperationType.Replace });
    AddСorrectOperations(nameof(EditWorkspaceTypeRequest.Description), new List<OperationType> { OperationType.Replace });
    AddСorrectOperations(nameof(EditWorkspaceTypeRequest.StartTime), new List<OperationType> { OperationType.Replace });
    AddСorrectOperations(nameof(EditWorkspaceTypeRequest.EndTime), new List<OperationType> { OperationType.Replace });
    AddСorrectOperations(nameof(EditWorkspaceTypeRequest.BookingRule), new List<OperationType> { OperationType.Replace });
    AddСorrectOperations(nameof(EditWorkspaceTypeRequest.IsActive), new List<OperationType> { OperationType.Replace });

    #endregion

    #region Name

    await AddFailureForPropertyIfAsync(
      nameof(EditWorkspaceTypeRequest.Name),
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
          async x => !await _workspaceTypeRepository.DoesNameExistAsync(x.value?.ToString().Trim()),
          "Workspace type name already exists."
        }
      }, CascadeMode.Stop);

    #endregion

    #region Description

    AddFailureForPropertyIf(
      nameof(EditWorkspaceTypeRequest.Description),
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
      nameof(EditWorkspaceTypeRequest.StartTime),
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
      nameof(EditWorkspaceTypeRequest.EndTime),
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
      nameof(EditWorkspaceTypeRequest.BookingRule),
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
      nameof(EditWorkspaceTypeRequest.IsActive),
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

  public EditWorkspaceTypeRequestValidator(
    IWorkspaceTypeRepository workspaceTypeRepository)
  {
    _workspaceTypeRepository = workspaceTypeRepository;

    RuleForEach(x => x.Operations)
      .CustomAsync(async (x, context, _) => await HandleInternalPropertyValidationAsync(x, context));
  }
}

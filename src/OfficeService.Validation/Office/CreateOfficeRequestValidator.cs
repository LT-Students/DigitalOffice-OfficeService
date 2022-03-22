using System.Linq;
using System.Text.RegularExpressions;
using FluentValidation;
using LT.DigitalOffice.OfficeService.Data.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Office;
using LT.DigitalOffice.OfficeService.Validation.Office.Interfaces;

namespace LT.DigitalOffice.OfficeService.Validation.Office
{
  public class CreateOfficeRequestValidator : AbstractValidator<CreateOfficeRequest>, ICreateOfficeRequestValidator
  {
    private readonly Regex _nameRegex = new(@"^\s+|\s+$|\s+(?=\s)");

    public CreateOfficeRequestValidator(
      IOfficeRepository _officeRepository)
    {
      When(x => !string.IsNullOrWhiteSpace(x.Name), () =>
      {
        RuleFor(x => x.Name)
          .MustAsync(async (name, _) => await _officeRepository.DoesNameExistAsync(_nameRegex.Replace(name, "")))
          .WithMessage("Name already exists.");
      });

      RuleFor(request => request.City)
        .NotEmpty().WithMessage("City must not be empty.")
        .MaximumLength(200).WithMessage("City's name is too long.");

      RuleFor(request => request.Address)
        .NotEmpty().WithMessage("Address must not be empty.");
    }
  }
}

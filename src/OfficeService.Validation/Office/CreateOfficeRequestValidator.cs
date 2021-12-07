using FluentValidation;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Office;
using LT.DigitalOffice.OfficeService.Validation.Office.Interfaces;

namespace LT.DigitalOffice.OfficeService.Validation.Office
{
  public class CreateOfficeRequestValidator : AbstractValidator<CreateOfficeRequest>, ICreateOfficeRequestValidator
  {
    public CreateOfficeRequestValidator()
    {
      RuleFor(request => request.Name)
        .NotEmpty().WithMessage("Name must not be empty.");

      RuleFor(request => request.City)
        .NotEmpty().WithMessage("City must not be empty.");

      RuleFor(request => request.Address)
        .NotEmpty().WithMessage("Address must not be empty.");

      RuleFor(request => request.Latitude)
        .NotEmpty().WithMessage("Latitude must not be empty.");

      RuleFor(request => request.Longitude)
        .NotEmpty().WithMessage("Longitude must not be empty.");
    }
  }
}

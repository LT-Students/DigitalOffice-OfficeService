using System.Text.RegularExpressions;
using FluentValidation;
using LT.DigitalOffice.OfficeService.DataLayer;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.OfficeService.Business.Office.Create
{
  public class CreateOfficeValidator : AbstractValidator<CreateOfficeRequest>
  {
    private readonly Regex _nameRegex = new(@"^\s+|\s+$|\s+(?=\s)");

    public CreateOfficeValidator(OfficeServiceDbContext dbContext)
    {
      When(r => !string.IsNullOrWhiteSpace(r.Name), () =>
      {
        RuleFor(r => r.Name)
          .MustAsync(async (name, ct) =>
            !await dbContext.Offices.AnyAsync(o => string.Equals(o.Name, _nameRegex.Replace(name, "")), ct))
          .WithMessage("Name already exists.");
      });

      RuleFor(r => r.City)
        .NotEmpty().WithMessage("City must not be empty.")
        .MaximumLength(200).WithMessage("City's name is too long.");

      RuleFor(r => r.Address)
        .NotEmpty().WithMessage("Address must not be empty.");
    }
  }
}

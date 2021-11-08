using FluentValidation;
using LT.DigitalOffice.OfficeService.Models.Dto.Requests.Office;
using LT.DigitalOffice.Kernel.Attributes;

namespace LT.DigitalOffice.OfficeService.Validation.Office.Interfaces
{
    [AutoInject]
    public interface ICreateOfficeRequestValidator : IValidator<CreateOfficeRequest>
    {
    }
}

namespace LT.DigitalOffice.OfficeService.Models.Dto.Requests.Office
{
  public record CreateOfficeRequest
  {
    public string Name { get; set; }
    public string City { get; set; }
    public string Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
  }
}

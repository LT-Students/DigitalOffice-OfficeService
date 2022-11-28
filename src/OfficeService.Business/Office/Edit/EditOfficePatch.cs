namespace LT.DigitalOffice.OfficeService.Business.Office.Edit
{
  public record EditOfficePatch
  {
    public string Name { get; set; }
    public string City { get; set; }
    public string Address { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public bool IsActive { get; set; }
  }
}

﻿using System;

namespace LT.DigitalOffice.OfficeService.Business.Office.Find
{
  public record OfficeInfo
  {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string City { get; set; }
    public string Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public bool IsActive { get; set; }
  }
}

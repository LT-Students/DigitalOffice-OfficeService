﻿using LT.DigitalOffice.Kernel.BrokerSupport.Attributes;
using LT.DigitalOffice.Kernel.BrokerSupport.Configurations;
using LT.DigitalOffice.Models.Broker.Common;

namespace LT.DigitalOffice.OfficeService
{
  public class RabbitMqConfig : BaseRabbitMqConfig
  {
    public string GetOfficesEndpoint { get; set; }
    public string CreateUserOfficeEndpoint { get; set; }
    public string DisactivateOfficeUserEndpoint { get; set; }
    public string FilterOfficesEndpoint { get; set; }
    public string CheckWorkspaceIsBookableEndpoint { get; set; }

    [AutoInjectRequest(typeof(ICheckUsersExistence))]
    public string CheckUsersExistenceEndpoint { get; set; }
  }
}

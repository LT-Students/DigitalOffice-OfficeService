﻿using System;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
using LT.DigitalOffice.Models.Broker.Requests.Office;
using LT.DigitalOffice.OfficeService.Broker.Requests;
using LT.DigitalOffice.OfficeService.DataLayer;
using LT.DigitalOffice.OfficeService.DataLayer.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.OfficeService.Broker.Consumers
{
  public class CheckWorkspaceIsBookableConsumer : IConsumer<ICheckWorkspaceIsBookableRequest>
  {
    private readonly OfficeServiceDbContext _dbContext;

    private async Task<DbWorkspace> GetWorkspacesAsync(Guid workspaceId)
    {
      return await _dbContext.Workspaces
        .Include(w => w.WorkspaceType)
        .FirstOrDefaultAsync(x =>
          x.Id == workspaceId
          && x.IsActive
          && x.WorkspaceType.IsActive);
    }

    private async Task<object> IsBookableAsync(Guid workspaceId)
    {
      DbWorkspace workspace = await GetWorkspacesAsync(workspaceId);

      return workspace is not null
        && workspace.IsBookable
        && workspace.WorkspaceType.BookingRule != (int)BookingRule.BookingForbidden;
    }

    public CheckWorkspaceIsBookableConsumer(
      OfficeServiceDbContext dbContext)
    {
      _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<ICheckWorkspaceIsBookableRequest> context)
    {
      object response = OperationResultWrapper.CreateResponse(IsBookableAsync, context.Message.WorkspaceId);

      await context.RespondAsync<IOperationResult<bool>>(response);
    }
  }
}

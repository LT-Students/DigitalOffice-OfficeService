using System;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
using LT.DigitalOffice.Models.Broker.Common;
using LT.DigitalOffice.OfficeService.Data.Workspace.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Db;
using LT.DigitalOffice.OfficeService.Models.Dto.Enums.Workspace;
using MassTransit;

namespace LT.DigitalOffice.OfficeService.Broker.Consumers
{
  public class CheckWorkspaceIsBookableConsumer : IConsumer<ICheckWorkspaceIsBookable>
  {
    private readonly IWorkspaceRepository _workspaceRepository;

    private async Task<object> IsBookableAsync(Guid workspaceId)
    {
      DbWorkspace workspace = await _workspaceRepository.GetAsync(workspaceId);
      if (workspace is not null)
      {
        return workspace.IsBookable && workspace.WorkspaceType.BookingRule != (int)BookingRule.BookingForbidden;
      }

      return false;
    }

    public CheckWorkspaceIsBookableConsumer(
      IWorkspaceRepository workspaceRepository)
    {
      _workspaceRepository = workspaceRepository;
    }

    public async Task Consume(ConsumeContext<ICheckWorkspaceIsBookable> context)
    {
      object response = OperationResultWrapper.CreateResponse(IsBookableAsync, context.Message.WorkspaceId);

      await context.RespondAsync<IOperationResult<bool>>(response);
    }
  }
}

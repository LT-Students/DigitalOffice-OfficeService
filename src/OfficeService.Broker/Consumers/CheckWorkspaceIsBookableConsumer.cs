using System;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
using LT.DigitalOffice.Models.Broker.Requests.Office;
using LT.DigitalOffice.OfficeService.Data.Workspace.Interfaces;
using LT.DigitalOffice.OfficeService.Models.Db;
using LT.DigitalOffice.OfficeService.Models.Dto.Enums.Workspace;
using MassTransit;

namespace LT.DigitalOffice.OfficeService.Broker.Consumers
{
  public class CheckWorkspaceIsBookableConsumer : IConsumer<ICheckWorkspaceIsBookableRequest>
  {
    private readonly IWorkspaceRepository _workspaceRepository;

    private async Task<object> IsBookableAsync(Guid workspaceId)
    {
      DbWorkspace workspace = await _workspaceRepository.GetAsync(workspaceId);

      return workspace is null
        ? false
        : workspace.IsBookable && workspace.WorkspaceType.BookingRule != (int)BookingRule.BookingForbidden;
    }

    public CheckWorkspaceIsBookableConsumer(
      IWorkspaceRepository workspaceRepository)
    {
      _workspaceRepository = workspaceRepository;
    }

    public async Task Consume(ConsumeContext<ICheckWorkspaceIsBookableRequest> context)
    {
      object response = OperationResultWrapper.CreateResponse(IsBookableAsync, context.Message.WorkspaceId);

      await context.RespondAsync<IOperationResult<bool>>(response);
    }
  }
}

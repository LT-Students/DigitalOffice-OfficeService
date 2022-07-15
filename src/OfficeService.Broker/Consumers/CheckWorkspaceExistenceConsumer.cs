using System;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
using LT.DigitalOffice.Models.Broker.Requests.Office;
using LT.DigitalOffice.OfficeService.Data.Workspace.Interfaces;
using MassTransit;

namespace LT.DigitalOffice.OfficeService.Broker.Consumers
{
  public class CheckWorkspaceExistenceConsumer : IConsumer<ICheckWorkspaceExistenceRequest>
  {
    private readonly IWorkspaceRepository _workspaceRepository;

    private async Task<object> GetWorkspaceExistenceInfoAsync(ICheckWorkspaceExistenceRequest requestId)
    {
      Guid? workspaceId = (await _workspaceRepository.GetAsync(requestId.WorkspaceId))?.Id;

      return workspaceId.HasValue 
        ? ICheckWorkspaceExistenceRequest.CreateObj(workspaceId.Value)
        : null;
    }

    public CheckWorkspaceExistenceConsumer(
      IWorkspaceRepository workspaceRepository)
    {
      _workspaceRepository = workspaceRepository;
    }

    public async Task Consume(ConsumeContext<ICheckWorkspaceExistenceRequest> context)
    {
      object response = OperationResultWrapper.CreateResponse(GetWorkspaceExistenceInfoAsync, context.Message);

      await context.RespondAsync<IOperationResult<ICheckWorkspaceExistenceRequest>>(response);
    }
  }
}

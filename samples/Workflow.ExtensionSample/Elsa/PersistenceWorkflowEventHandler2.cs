using System.Threading;
using System.Threading.Tasks;
using Elsa.Persistence;
using Elsa.Services.Models;
using Elsa.WorkflowEventHandlers;

namespace WorkflowExtension.Elsa
{
    public class PersistenceWorkflowEventHandler2 : PersistenceWorkflowEventHandler
    {
        private readonly IWorkflowInstanceStore workflowInstanceStore;

        public PersistenceWorkflowEventHandler2(IWorkflowInstanceStore workflowInstanceStore)
            : base(workflowInstanceStore)
        {
            this.workflowInstanceStore = workflowInstanceStore;
        }

        public override async Task WorkflowInvokedAsync(WorkflowExecutionContext workflowExecutionContext, CancellationToken cancellationToken)
        {
            await base.WorkflowInvokedAsync(workflowExecutionContext, cancellationToken);
            await workflowInstanceStore.DeleteAsync(workflowExecutionContext.Workflow.Id, cancellationToken);
        }
    }
}

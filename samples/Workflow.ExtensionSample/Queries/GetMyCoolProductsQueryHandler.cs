using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Models;
using Elsa.Serialization;
using Elsa.Serialization.Formatters;
using Elsa.Services;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WorkflowExtension.Model;

namespace WorkflowExtension.Queries
{
    public class GetMyCoolProductsQueryHandler : IRequestHandler<GeMyCoolProductsQuery, GetMyCoolProductsResponse>
    {
        private readonly IWorkflowSerializer _workflowSerializer;
        private readonly Func<IWorkflowInvoker> _workflowInvokerFactory;

        public GetMyCoolProductsQueryHandler(Func<IWorkflowInvoker> workflowInvoker, IWorkflowSerializer workflowSerializer)
        {
            _workflowInvokerFactory = workflowInvoker;
            _workflowSerializer = workflowSerializer;
        }
        public async Task<GetMyCoolProductsResponse> Handle(GeMyCoolProductsQuery request, CancellationToken cancellationToken)
        {
            var result = new GetMyCoolProductsResponse { Products = request.Ids.Select(x => new MyCoolProduct { Id = x, Name = $"MyCoolProduct{x}" }).ToList() };

            var input = new Variables { ["Request"] = new Variable(request), ["Response"] = new Variable(result) };
            var workflow = LoadWorkflow();

            var context = await _workflowInvokerFactory().StartAsync(workflow, input);

            //TODO: Move out of here into dedicated Generic type
            //Merge response from external service to the actual result
            var workflowResult  = context.CurrentScope.GetVariable("Result");
            if(workflowResult != null && workflowResult is JObject extensionResponseJson)
            {
                var externextensionResponse = extensionResponseJson.ToObject<GetMyCoolProductsResponse>();
                if (externextensionResponse != null)
                {
                    foreach (var product in result.Products)
                    {
                        var otherProduct = externextensionResponse.Products.FirstOrDefault(x => x.Id == product.Id);
                        if (otherProduct != null)
                        {
                            product.MergefromOther(otherProduct);
                        }
                    }
                }
            }

            return result;
        }

        private WorkflowDefinitionVersion LoadWorkflow()
        {
            var workflow = File.ReadAllText("GetMyCoolProductsWorkflow.json"); 
            return _workflowSerializer.Deserialize<WorkflowDefinitionVersion>(workflow, JsonTokenFormatter.FormatName);
        }
    }
}

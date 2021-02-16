using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Extensions;
using Elsa.Results;
using Elsa.Services;
using Elsa.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WorkflowExtension.Model;
using WorkflowExtension.Queries;

namespace WorkflowExtension.Activities
{
    [ActivityDefinition(Category = "VirtoCommerce", Description = "Evaluated products inventory for given products")]
    public class EvalProductsPricesActivity : Activity
    {

        private readonly IWorkflowExpressionEvaluator _expressionEvaluator;
        public EvalProductsPricesActivity(IWorkflowExpressionEvaluator expressionEvaluator)
        {
            _expressionEvaluator = expressionEvaluator;
        }

        [ActivityProperty(Hint = "The request")]
        public WorkflowExpression<GeMyCoolProductsQuery> Request
        {
            get => GetState(() => new WorkflowExpression<GeMyCoolProductsQuery>(LiteralEvaluator.SyntaxName, ""));
            set => SetState(value);
        }
        [ActivityProperty(Hint = "The response")]
        public WorkflowExpression<GetMyCoolProductsResponse> Response
        {
            get => GetState(() => new WorkflowExpression<GetMyCoolProductsResponse>(LiteralEvaluator.SyntaxName, ""));
            set => SetState(value);
        }

        protected override async Task<ActivityExecutionResult> OnExecuteAsync(WorkflowExecutionContext context, CancellationToken cancellationToken)
        {
            var response = await _expressionEvaluator.EvaluateAsync(Response, context, cancellationToken);
            foreach (var product in response?.Products ?? Array.Empty<MyCoolProduct>())
            {
                product.Price = 10.99m;
            }
            return Done();
        }
    }
}
